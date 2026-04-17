// Fill out your copyright notice in the Description page of Project Settings.


#include "MoverCm.h"
#include "Math/UnrealMathUtility.h"

// Sets default values for this component's properties
UMoverCm::UMoverCm()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}


// Called when the game starts
void UMoverCm::BeginPlay()
{
	Super::BeginPlay();
	

	startLocation = GetOwner()->GetActorLocation();
	SetshouldMove(false);
}


// Called every frame
void UMoverCm::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	
	FVector currentLocation = GetOwner()->GetActorLocation();

	reachedTarget = currentLocation.Equals(targetLocation);

	if (!reachedTarget) 
	{
		float speed = moveOffset.Length() / moveTime;

		FVector newLocation = FMath::VInterpConstantTo(currentLocation, targetLocation, DeltaTime, speed);

		GetOwner()->SetActorLocation(newLocation);
	}
}

bool UMoverCm::GetshouldMove()
{
	return shouldMove;
}

void UMoverCm::SetshouldMove(bool newShouldMove)
{
	shouldMove = newShouldMove;

	if (shouldMove)
	{
		targetLocation = startLocation + moveOffset;
	}
	else
	{
		targetLocation = startLocation;
	}
}

