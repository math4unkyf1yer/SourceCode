// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "ShooterSamGameMode.generated.h"

/**
 *  Simple GameMode for a third person game
 */
UCLASS(abstract)
class AShooterSamGameMode : public AGameModeBase
{
	GENERATED_BODY()

public:
	
	/** Constructor */
	AShooterSamGameMode();
	protected:
	virtual void BeginPlay() override;
	
	
	void Lost();
	
	void RestartingLevel();
	void SpawnNextLevelPortal();
	void CheckForLevelComplete();
public:
	UFUNCTION()
	void NotifyEnemyDied();
	
	UFUNCTION()
	void NotifyPlayerDied();
	
	FTimerHandle TimerHandle;
	
	UPROPERTY(EditAnywhere, Category= "Level")
	TSubclassOf<class ANextLevelPortal> NextLevelPortalClass;
	
	UPROPERTY(EditAnywhere, Category= "Level")
	TArray<FName> LevelOrder;
	
	int32 CurrentLevelIndex = 0;
	
	//variables
	int enemies;
};



