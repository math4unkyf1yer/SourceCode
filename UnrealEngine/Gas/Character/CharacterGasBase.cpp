// Fill out your copyright notice in the Description page of Project Settings.


#include "CharacterGasBase.h"
#include "CharacterGasBase.h"

#include "Components/CapsuleComponent.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "GasLearning/GameplayAbilitySystem/AbilityAttributes/BasicAttributeSet.h"

// Sets default values
ACharacterGasBase::ACharacterGasBase()
{
 	// Set this character to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	//ADD THE ABILIOTY SYSTEM
	AbilitySystemComponent = CreateDefaultSubobject<UAbilitySystemComponent>(TEXT("AbilitySystemComponent"));
	AbilitySystemComponent->SetIsReplicated(true);//multiplayer
	AbilitySystemComponent->SetReplicationMode(ReplicationMode);//mixed some are replicated for multiplayer none for enemy
	
	//Ability System Attribute
	BasicAtSet = CreateDefaultSubobject<UBasicAttributeSet>(TEXT("BasicAttributeSet"));
	
	//need to make character capsule
	GetCapsuleComponent()->InitCapsuleSize(35.0f,90.0f);
	
	bUseControllerRotationPitch = false;
	bUseControllerRotationYaw = false;
	bUseControllerRotationRoll = false;
	
	
	//MOVEMENT BASE CHARACTEr
	GetCharacterMovement()->bOrientRotationToMovement = true;
	GetCharacterMovement()->RotationRate = FRotator(0.0f, 500.0f, 0.0f);
	
	GetCharacterMovement()->JumpZVelocity = 500.0f;
	GetCharacterMovement()-> AirControl = 0.35f;
	GetCharacterMovement()->MaxWalkSpeed = 500.0f;
	GetCharacterMovement()->MinAnalogWalkSpeed= 20.0f;
	GetCharacterMovement()->BrakingDecelerationWalking = 2000.0f;
	GetCharacterMovement()->BrakingDecelerationFalling = 1500.0f;
}

// Called when the game starts or when spawned
void ACharacterGasBase::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void ACharacterGasBase::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

// Called to bind functionality to input
void ACharacterGasBase::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

	
}

void ACharacterGasBase::OnRep_PlayerState()
{
	Super::OnRep_PlayerState();
	
	if (AbilitySystemComponent)
	{
		AbilitySystemComponent->InitAbilityActorInfo(this,this);
	}
}

void ACharacterGasBase::PossessedBy(AController* NewController)
{
	Super::PossessedBy(NewController);
	if (AbilitySystemComponent)
	{
		AbilitySystemComponent->InitAbilityActorInfo(this,this);
	}
}

UAbilitySystemComponent* ACharacterGasBase::GetAbilitySystemComponent() const
{
	return AbilitySystemComponent;
}

