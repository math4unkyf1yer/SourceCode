// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include  "ShooterSamCharacter.h"
#include "Components/StaticMeshComponent.h"
#include "Components/BoxComponent.h"
#include "HealthPickUp.generated.h"

UCLASS()
class SHOOTERSAM_API AHealthPickUp : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	AHealthPickUp();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;
	
	
	UPROPERTY(EditAnywhere)
	UBoxComponent* BoxComp;
	
	UPROPERTY(EditAnywhere)
	UStaticMeshComponent* StaticMeshComp;
	
	UPROPERTY(EditAnywhere)
	float HealAmount;
	
	UFUNCTION(BlueprintCallable, Category="Health")
	void HealPlayer(float Amount,AActor* OtherActor);
	
	UFUNCTION()
	void OnBoxBeginOverlap(UPrimitiveComponent* OverlappedComp,AActor* OtherActor,UPrimitiveComponent* OtherComp,int32 OtherBodyIndex,bool bFromSweep,const FHitResult& SweepResult);

};
