// Fill out your copyright notice in the Description page of Project Settings.


#include "Lock.h"

// Sets default values
ALock::ALock()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	rootComp = CreateDefaultSubobject<USceneComponent>(TEXT("Root Comp"));
	SetRootComponent(rootComp);

	triggerComp = CreateDefaultSubobject<UTriggerCollider>(TEXT("Trigger Comp"));
	triggerComp->SetupAttachment(rootComp);

	keyItemMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("key Item Mesh"));
	keyItemMesh->SetupAttachment(rootComp);

	Tags.Add("Lock");
}


// Called when the game starts or when spawned
void ALock::BeginPlay()
{
	Super::BeginPlay();
	SetItemPlace(false);

}

// Called every frame
void ALock::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

bool ALock::GetItemPlace() 
{
	return itemPlace;
}
void ALock::SetItemPlace(bool value) 
{
	itemPlace = value;
	triggerComp->Trigger(value);
	keyItemMesh->SetVisibility(itemPlace);
}


