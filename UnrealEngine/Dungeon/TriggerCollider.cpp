// Fill out your copyright notice in the Description page of Project Settings.


#include "TriggerCollider.h"

UTriggerCollider::UTriggerCollider()
{
	PrimaryComponentTick.bCanEverTick = true;
	
	
}

void UTriggerCollider::BeginPlay()
{
	Super::BeginPlay();

	if (moverActor) 
	{
		moverClass = moverActor->FindComponentByClass<UMoverCm>();
		if (moverClass) 
		{
			UE_LOG(LogTemp, Display, TEXT("Found mover class in trigger"));
			
		}
	}

	if (isPressurePlate) 
	{
		OnComponentBeginOverlap.AddDynamic(this, &UTriggerCollider::OnOverlapBegin);
		OnComponentEndOverlap.AddDynamic(this, &UTriggerCollider::OnOverlapEnd);
	}
}


void UTriggerCollider::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);
	
}

void UTriggerCollider::OnOverlapBegin(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	if (OtherActor && OtherActor->ActorHasTag("PressurePlateActivator") && moverClass) 
	{
		activatorCount++;
		if (!isTriggered) {
			Trigger(true);
		}
	}
}

void UTriggerCollider::OnOverlapEnd(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex)
{
	if (OtherActor && OtherActor->ActorHasTag("PressurePlateActivator") && moverClass)
	{
		activatorCount--;
		if (isTriggered && activatorCount == 0) {
			Trigger(false);
		}
	}
}

void UTriggerCollider::Trigger(bool isTrigger)
{
	isTriggered = isTrigger;
	moverClass->SetshouldMove(isTrigger);
}


