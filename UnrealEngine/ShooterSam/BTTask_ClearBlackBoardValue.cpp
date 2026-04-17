// Fill out your copyright notice in the Description page of Project Settings.


#include "BTTask_ClearBlackBoardValue.h"

#include "BehaviorTree/BlackboardComponent.h"

UBTTask_ClearBlackBoardValue:: UBTTask_ClearBlackBoardValue()
{
	NodeName = TEXT("ClearBlackBoardValue");
}

EBTNodeResult::Type UBTTask_ClearBlackBoardValue::ExecuteTask(UBehaviorTreeComponent& OwnerComp, uint8* NodeMemory)
{
	Super::ExecuteTask(OwnerComp, NodeMemory);
	
	UBlackboardComponent* blackBoard = OwnerComp.GetBlackboardComponent();
	if (blackBoard)
	{
		blackBoard->ClearValue(GetSelectedBlackboardKey());
	}
	return EBTNodeResult::Succeeded;
}
