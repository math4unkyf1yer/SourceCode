// Fill out your copyright notice in the Description page of Project Settings.


#include "CollectibleClass.h"

// Sets default values
ACollectibleClass::ACollectibleClass()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	Tags.Add("CollectibleItem");
}

// Called when the game starts or when spawned
void ACollectibleClass::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void ACollectibleClass::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

