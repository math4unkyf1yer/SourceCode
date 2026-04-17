// Fill out your copyright notice in the Description page of Project Settings.


#include "HealthBarWidgetEnemy.h"

#include "Components/ProgressBar.h"


void UHealthBarWidgetEnemy::SetHealthBarPercentage(float NewPercent)
{
	if (NewPercent >= 0.0 && NewPercent <= 1.0f)
	{
		HealthBar->SetPercent(NewPercent);
	}
}