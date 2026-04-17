// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "TriggerCollider.h"
#include "Components/StaticMeshComponent.h"

#include "Lock.generated.h"

UCLASS()
class DUNGEONESCAPE_API ALock : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ALock();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;


	UPROPERTY(VisibleAnywhere)
	USceneComponent* rootComp;

	UPROPERTY(VisibleAnywhere)
	UTriggerCollider* triggerComp;

	UPROPERTY(VisibleAnywhere)
	UStaticMeshComponent* keyItemMesh;

	UPROPERTY(EditAnywhere)
	FString keyItemName;

private:
	UPROPERTY(VisibleAnywhere)
	bool itemPlace = false;

public:
	bool GetItemPlace();
	void SetItemPlace(bool value);

	FVector testing = FVector(10, 20, 30);
};
