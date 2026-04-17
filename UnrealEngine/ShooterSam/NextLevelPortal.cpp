// Fill out your copyright notice in the Description page of Project Settings.


#include "NextLevelPortal.h"

#include "ShooterSamCharacter.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"
#include "ShooterSamGameMode.h"
#include "ShooterSamCharacter.h"
#include "Kismet/GameplayStatics.h"


// Sets default values
ANextLevelPortal::ANextLevelPortal()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	BoxComp = CreateDefaultSubobject<UBoxComponent>(FName("BoxComp"));
	RootComponent = BoxComp;
	
	//collision overlaped function 
	BoxComp->OnComponentBeginOverlap.AddDynamic(this,&ANextLevelPortal::OnBoxBeginOverlaped);
	
	StaticMeshComp = CreateDefaultSubobject<UStaticMeshComponent>(FName("StaticMesh"));
	StaticMeshComp->SetupAttachment(RootComponent);
	
	StaticMeshComp->SetCollisionEnabled(ECollisionEnabled::NoCollision);
}

// Called when the game starts or when spawned
void ANextLevelPortal::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void ANextLevelPortal::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

void ANextLevelPortal::OnBoxBeginOverlaped(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	if (OtherActor)
	{
		UE_LOG(LogTemp,Warning,TEXT("took portal change level"));
		
		AShooterSamCharacter* character = Cast<AShooterSamCharacter>(OtherActor);
		AShooterSamGameMode* Gamemode = Cast<AShooterSamGameMode>(UGameplayStatics::GetGameMode(this));
		
		if (Gamemode && character && character->isPlayer)
		{
			int32 NexIndex = Gamemode->CurrentLevelIndex+1;
			if (Gamemode->LevelOrder.IsValidIndex(NexIndex))
			{
				UGameplayStatics::OpenLevel(this,Gamemode->LevelOrder[NexIndex]);
			}else
			{
				UE_LOG(LogTemp,Warning,TEXT("Invalid level index"));
				
			}
		}
		
		UGameplayStatics::OpenLevel(this,FName("Level1"));
	}
}


