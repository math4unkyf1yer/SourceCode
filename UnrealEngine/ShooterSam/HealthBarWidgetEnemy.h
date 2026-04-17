// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "Components//ProgressBar.h"
#include "HealthBarWidgetEnemy.generated.h"

/**
 * 
 */
UCLASS()
class SHOOTERSAM_API UHealthBarWidgetEnemy : public UUserWidget
{
	GENERATED_BODY()
	
public:
	UPROPERTY(EditAnywhere, meta = (BindWidgetOptional))
	UProgressBar* HealthBar;
	
	void SetHealthBarPercentage(float NewPercent);
	
};

