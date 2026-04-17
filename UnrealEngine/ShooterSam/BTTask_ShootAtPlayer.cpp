// Fill out your copyright notice in the Description page of Project Settings.


#include "BTTask_ShootAtPlayer.h"

#include "ShooterAI.h"


UBTTask_ShootAtPlayer::UBTTask_ShootAtPlayer()
{
	NodeName = TEXT("Shoot at player task");
}

EBTNodeResult::Type UBTTask_ShootAtPlayer::ExecuteTask(UBehaviorTreeComponent& OwnerComp, uint8* NodeMemory)
{
	Super::ExecuteTask(OwnerComp, NodeMemory);
	
	EBTNodeResult::Type result = EBTNodeResult::Failed;
	
	AShooterAI* OwnerController = Cast<AShooterAI>(OwnerComp.GetAIOwner());
	if (OwnerController)
	{
		AShooterSamCharacter* OwnerCharacter = OwnerController->MyCharacter;
		AShooterSamCharacter* PlayerCharacter = OwnerController->PlayerCharacter;
		
		if (OwnerCharacter && PlayerCharacter && PlayerCharacter->isAlive)
		{
			OwnerCharacter->Shoot();
			result = EBTNodeResult::Succeeded;
		}else
		{
			result = EBTNodeResult::Failed;
		}
		
	}
	return  result;
}

