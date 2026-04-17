// Copyright Epic Games, Inc. All Rights Reserved.

#include "ShooterSamGameMode.h"

#include "NextLevelPortal.h"
#include "ShooterSamCharacter.h"
#include "Kismet/GameplayStatics.h"
#include "ShooterAI.h"

AShooterSamGameMode::AShooterSamGameMode()
{
	// stu
}

void AShooterSamGameMode::BeginPlay()
{
	Super::BeginPlay();
	//which level
	FString CurrentLevel = GetWorld()->GetMapName();
	CurrentLevel.RemoveFromStart(GetWorld()->StreamingLevelsPrefix);
	
	for (int32 i = 0; i < LevelOrder.Num(); i++)
	{
		if (LevelOrder[i].ToString() == CurrentLevel)
		{
			CurrentLevelIndex = i;
			break;
		}
	}
	
	//begin play
	AShooterSamCharacter* Player = Cast<AShooterSamCharacter>(UGameplayStatics::GetPlayerPawn(GetWorld(),0));
	
	TArray<AActor*> ShooterAiActors;
	
	UGameplayStatics::GetAllActorsOfClass(GetWorld(),AShooterAI::StaticClass(),ShooterAiActors);
	
	enemies = ShooterAiActors.Num();
	
	for (int32 i = 0; i < ShooterAiActors.Num(); i++)
	{
		AActor* Actor = ShooterAiActors[i];
		AShooterAI* ShooterAI = Cast<AShooterAI>(Actor);
		if (ShooterAI)
		{
			ShooterAI->StartBehaviorTree(Player);
			
			UE_LOG(LogTemp, Error, TEXT("%s Starting behavior tree"),*ShooterAI->GetActorNameOrLabel());
		}
	}
}


void AShooterSamGameMode::NotifyPlayerDied()
{
     Lost();
}

void AShooterSamGameMode::NotifyEnemyDied()
{
	CheckForLevelComplete();
}

void AShooterSamGameMode::RestartingLevel()
{
	UGameplayStatics::OpenLevel(this,FName("Test_level"));
}

void AShooterSamGameMode::SpawnNextLevelPortal()
{
	UE_LOG(LogTemp, Warning, TEXT("SpawnNextLevelPortal Win"));
	FVector SpawnLocation(0.f,0.f,0.f);
	FRotator SpawnRotation = FRotator::ZeroRotator;
	
	if (!NextLevelPortalClass)
	{
		return;
	}
	
	GetWorld()->SpawnActor<ANextLevelPortal>(NextLevelPortalClass,SpawnLocation,SpawnRotation);
}

void AShooterSamGameMode::CheckForLevelComplete()
{
	enemies--;
	if (enemies == 0)
	{
		SpawnNextLevelPortal();
	}
}

void AShooterSamGameMode::Lost()
{
	UE_LOG(LogTemp, Warning, TEXT("Lose"));
	GetWorld()->GetTimerManager().SetTimer(TimerHandle,this,&AShooterSamGameMode::RestartingLevel,2.0f,false);
}
