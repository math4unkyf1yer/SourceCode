// Fill out your copyright notice in the Description page of Project Settings.


#include "Gun.h"

#include "ShooterSamCharacter.h"
#include "Kismet/GameplayStatics.h"

#define PI 3.12159f

#define LogReplace(text,value) UE_LOG(LogTemp,Display,TEXT(text),value)



// Sets default values
AGun::AGun()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	sceneComp = CreateDefaultSubobject<USceneComponent>(TEXT("SceneRoot"));
	skeletonGun = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("meshGun"));
	SetRootComponent(sceneComp);
	skeletonGun->SetupAttachment(sceneComp);


	MuzzleFlashParticleSystem = CreateDefaultSubobject<UNiagaraComponent>(TEXT("MuzzleFlash"));
	MuzzleFlashParticleSystem->SetupAttachment(skeletonGun);

}

// Called when the game starts or when spawned
void AGun::BeginPlay()
{
	Super::BeginPlay();
	MuzzleFlashParticleSystem->Deactivate();
	
}

// Called every frame
void AGun::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

void AGun::FuckingShooting()
{
	if (!CharacterRef)
	{
		CharacterRef = Cast<AShooterSamCharacter>(GetOwner());	
	}
	if (CharacterRef && CharacterRef->isRunning == false)
	{
		UE_LOG(LogTemp,Error,TEXT("Gun has player character"));
		MuzzleFlashParticleSystem->Activate(true);
		UGameplayStatics::PlaySoundAtLocation(GetWorld(), GunSound, GetActorLocation());
	

		if (OwnerController) {
			FVector viewPointLocation;
			FVector endPoint;
			FRotator viewPointRotator;
			OwnerController->GetPlayerViewPoint(viewPointLocation, viewPointRotator);

			endPoint = viewPointLocation + viewPointRotator.Vector() * maxRange;

			FHitResult hitResult;
			FCollisionQueryParams params;
			params.AddIgnoredActor(this);
			params.AddIgnoredActor(GetOwner());

			bool hitSomething = GetWorld()->LineTraceSingleByChannel(hitResult, viewPointLocation, endPoint, ECC_GameTraceChannel2, params);

			if (hitSomething) {

				UNiagaraFunctionLibrary::SpawnSystemAtLocation(GetWorld(), impactParticleSystem, hitResult.ImpactPoint,hitResult.ImpactPoint.Rotation());
			
				UGameplayStatics::PlaySoundAtLocation(GetWorld(), ImpactSound, hitResult.ImpactPoint);

				AActor* hitActor = hitResult.GetActor();
				if (hitActor) {
					UGameplayStatics::ApplyDamage(hitActor, bulletDamage, OwnerController, this,UDamageType::StaticClass());
				}
			}
		}
	}

}



