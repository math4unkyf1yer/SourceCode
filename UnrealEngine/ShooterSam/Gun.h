// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "NiagaraFunctionLibrary.h"
#include "NiagaraComponent.h"
#include "Gun.generated.h"

class AShooterSamCharacter;

UCLASS()
class SHOOTERSAM_API AGun : public AActor
{
	GENERATED_BODY()

	
public:	
	// Sets default values for this actor's properties
	AGun();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	UPROPERTY(VisibleAnywhere)
	USceneComponent* sceneComp;

	UPROPERTY(VisibleAnywhere)
	UNiagaraComponent* MuzzleFlashParticleSystem;

	UPROPERTY(EditAnywhere)
	UNiagaraSystem* impactParticleSystem;

	UPROPERTY(VisibleAnywhere)
	USkeletalMeshComponent* skeletonGun;

	AController* OwnerController;
	
	UPROPERTY(EditAnywhere)
	USoundBase* ImpactSound;
	
	UPROPERTY(EditAnywhere)
	USoundBase* GunSound;

	UPROPERTY(EditAnywhere)
	float maxRange = 10000;

	UPROPERTY(EditAnywhere)
	float bulletDamage = 10;
	
	AShooterSamCharacter* CharacterRef;

public:
	void FuckingShooting();

};
