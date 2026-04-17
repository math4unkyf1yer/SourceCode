// Copyright Epic Games, Inc. All Rights Reserved.

#include "ShooterSamCharacter.h"
#include "Engine/LocalPlayer.h"
#include "Camera/CameraComponent.h"
#include "Components/CapsuleComponent.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "GameFramework/SpringArmComponent.h"
#include "Gun.h"
#include "Kismet/GameplayStatics.h"
#include "GameFramework/Controller.h"
#include "EnhancedInputComponent.h"
#include "EnhancedInputSubsystems.h"
#include "HealthBarWidget.h"
#include "InputActionValue.h"
#include "ShooterSam.h"
#include "ShooterSamGameMode.h"
#include "ShooterSamPlayerController.h"


AShooterSamCharacter::AShooterSamCharacter()
{
	// Set size for collision capsule
	GetCapsuleComponent()->InitCapsuleSize(42.f, 96.0f);
		
	// Don't rotate when the controller rotates. Let that just affect the camera.
	bUseControllerRotationPitch = false;
	bUseControllerRotationYaw = false;
	bUseControllerRotationRoll = false;

	// Configure character movement
	GetCharacterMovement()->bOrientRotationToMovement = true;
	GetCharacterMovement()->RotationRate = FRotator(0.0f, 500.0f, 0.0f);

	// Note: For faster iteration times these variables, and many more, can be tweaked in the Character Blueprint
	// instead of recompiling to adjust them
	GetCharacterMovement()->JumpZVelocity = 500.f;
	GetCharacterMovement()->AirControl = 0.35f;
	GetCharacterMovement()->MaxWalkSpeed = WalkSpeed;
	GetCharacterMovement()->MinAnalogWalkSpeed = 20.f;
	GetCharacterMovement()->BrakingDecelerationWalking = 2000.f;
	GetCharacterMovement()->BrakingDecelerationFalling = 1500.0f;

	// Create a camera boom (pulls in towards the player if there is a collision)
	CameraBoom = CreateDefaultSubobject<USpringArmComponent>(TEXT("CameraBoom"));
	CameraBoom->SetupAttachment(RootComponent);
	CameraBoom->TargetArmLength = 400.0f;
	CameraBoom->bUsePawnControlRotation = true;

	// Create a follow camera
	FollowCamera = CreateDefaultSubobject<UCameraComponent>(TEXT("FollowCamera"));
	FollowCamera->SetupAttachment(CameraBoom, USpringArmComponent::SocketName);
	FollowCamera->bUsePawnControlRotation = false;
	
	

	// Note: The skeletal mesh and anim blueprint references on the Mesh component (inherited from Character) 
	// are set in the derived blueprint asset named ThirdPersonCharacter (to avoid direct content references in C++)
}

void AShooterSamCharacter::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	// Set up action bindings
	if (UEnhancedInputComponent* EnhancedInputComponent = Cast<UEnhancedInputComponent>(PlayerInputComponent)) {
		
		// Jumping
		EnhancedInputComponent->BindAction(JumpAction, ETriggerEvent::Started, this, &ACharacter::Jump);
		EnhancedInputComponent->BindAction(JumpAction, ETriggerEvent::Completed, this, &ACharacter::StopJumping);
		
		//running
		EnhancedInputComponent->BindAction(runAction,ETriggerEvent::Started, this, &AShooterSamCharacter::StartRunning);
		EnhancedInputComponent->BindAction(runAction,ETriggerEvent::Completed,this,&AShooterSamCharacter::StopRunning);

		// Moving
		EnhancedInputComponent->BindAction(MoveAction, ETriggerEvent::Triggered, this, &AShooterSamCharacter::Move);
		EnhancedInputComponent->BindAction(MouseLookAction, ETriggerEvent::Triggered, this, &AShooterSamCharacter::Look);

		// Looking
		EnhancedInputComponent->BindAction(LookAction, ETriggerEvent::Triggered, this, &AShooterSamCharacter::Look);

		//shoot weapon
		EnhancedInputComponent->BindAction(shootAction, ETriggerEvent::Started, this, &AShooterSamCharacter::Shoot);
	}
	else
	{
		UE_LOG(LogShooterSam, Error, TEXT("'%s' Failed to find an Enhanced Input component! This template is built to use the Enhanced Input system. If you intend to use the legacy system, then you will need to update this C++ file."), *GetNameSafe(this));
	}
}

void AShooterSamCharacter::BeginPlay()
{
	Super::BeginPlay();

	health = maxHealth;
	Stamina = maxStamina;
	
	UpdateHealthBar();

	OnTakeAnyDamage.AddDynamic(this, &AShooterSamCharacter::OnDamageTaken);

	GetMesh()->HideBoneByName("weapon_r",EPhysBodyOp::PBO_None);

	Gun = GetWorld()->SpawnActor<AGun>(GunClass);
	if (Gun) {
		Gun->SetOwner(this);
		Gun->AttachToComponent(GetMesh(), FAttachmentTransformRules::KeepRelativeTransform, TEXT("WeaponSocket"));

		Gun->OwnerController = GetController();
	}
	
	 WidgetComp = FindComponentByClass<UWidgetComponent>();
	if (WidgetComp)
	{
		
		UUserWidget* Widget = WidgetComp->GetUserWidgetObject();
		if (Widget)
		{
			HealthBarWidget = Cast<UHealthBarWidget>(Widget);	
		}
	}

}

void AShooterSamCharacter::Move(const FInputActionValue& Value)
{
	// input is a Vector2D
	FVector2D MovementVector = Value.Get<FVector2D>();


	// route the input
	DoMove(MovementVector.X, MovementVector.Y);
}

void AShooterSamCharacter::Look(const FInputActionValue& Value)
{
	// input is a Vector2D
	FVector2D LookAxisVector = Value.Get<FVector2D>();

	// route the input
	DoLook(LookAxisVector.X, LookAxisVector.Y);
}

void AShooterSamCharacter::DoMove(float Right, float Forward)
{
	if (GetController() != nullptr)
	{
		// find out which way is forward
		const FRotator Rotation = GetController()->GetControlRotation();
		const FRotator YawRotation(0, Rotation.Yaw, 0);

		// get forward vector
		const FVector ForwardDirection = FRotationMatrix(YawRotation).GetUnitAxis(EAxis::X);

		// get right vector 
		const FVector RightDirection = FRotationMatrix(YawRotation).GetUnitAxis(EAxis::Y);

		// add movement 
		AddMovementInput(ForwardDirection, Forward);
		AddMovementInput(RightDirection, Right);
	}
}

void AShooterSamCharacter::StartRunning()
{
	Running();
}
void AShooterSamCharacter::StopRunning()
{
	Walking();
}

void AShooterSamCharacter::Running()
{
	GetCharacterMovement()->MaxWalkSpeed = MaxWalkSpeed;
	isRunning = true;
}

void AShooterSamCharacter::Walking()
{
	GetCharacterMovement()->MaxWalkSpeed = WalkSpeed;
	isRunning = false;
}

void AShooterSamCharacter::Shoot()
{
	Gun->FuckingShooting();
}

void AShooterSamCharacter::DoLook(float Yaw, float Pitch)
{
	if (GetController() != nullptr)
	{
		// add yaw and pitch input to controller
		AddControllerYawInput(Yaw);
		AddControllerPitchInput(Pitch);
	}
}

void AShooterSamCharacter::DoJumpStart()
{
	// signal the character to jump
	Jump();
}

void AShooterSamCharacter::DoJumpEnd()
{
	// signal the character to stop jumping
	StopJumping();
}
void AShooterSamCharacter::UpdateHealthBar()
{
	AShooterSamPlayerController* PlayerController = Cast<AShooterSamPlayerController>(GetController());
	if (PlayerController)
	{
		UE_LOG(LogTemp, Warning, TEXT("Health bar updated"));
		float currentHealth = health/maxHealth;
		PlayerController->HudWidget->SetHealthBarPercentage(currentHealth);
	}
	if (HealthBarWidget)
	{
		float currentHealth = health/maxHealth;
		UE_LOG(LogTemp,Display,TEXT("IT WORKS"));
		HealthBarWidget->SetHealthBarPercentage(currentHealth);
	}
	

}

void AShooterSamCharacter::OnDamageTaken(AActor* DamagedActor, float Damage, const UDamageType* DamageType, AController* InstigatedBy, AActor* DamageCauser)
{
	if (isAlive) {
		health -= Damage;
		UpdateHealthBar();
		if (health > 0) {
			UE_LOG(LogTemp, Display, TEXT("health remaining %f"), health);
		}
		else {

			isAlive = false;
			health = 0;
			GetCapsuleComponent()->SetCollisionEnabled(ECollisionEnabled::NoCollision);
			DetachFromControllerPendingDestroy();
			
			if (AShooterSamGameMode* GM = Cast<AShooterSamGameMode>(UGameplayStatics::GetGameMode(GetWorld())))
			{
				if (isPlayer)
				{
				    GM->NotifyPlayerDied();
					
				}else
				{
					GM->NotifyEnemyDied();
					WidgetComp->SetVisibility(false);
				}	
			}
		}
	}
}

bool AShooterSamCharacter::CanHeal()
{
	if (health < maxHealth)
	{
		return true;
	}else
	{
		return false;
	}
}

void AShooterSamCharacter::IncreaseHealth(float Increase)
{
	
	health += Increase;
	health = FMath::Clamp(health, 0, maxHealth);

	UpdateHealthBar();
}


