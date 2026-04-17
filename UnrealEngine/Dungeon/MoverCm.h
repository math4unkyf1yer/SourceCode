// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "MoverCm.generated.h"


UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class DUNGEONESCAPE_API UMoverCm : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UMoverCm();

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

		
	FVector startLocation;
	FVector targetLocation;

	UPROPERTY(EditAnywhere)
	FVector moveOffset;

	UPROPERTY(EditAnywhere)
	float moveTime;

	bool GetshouldMove();
	void SetshouldMove(bool newShouldMove);

private:
	UPROPERTY(VisibleAnywhere)
	bool shouldMove = false;


	UPROPERTY(VisibleAnywhere)
	bool reachedTarget;
};
