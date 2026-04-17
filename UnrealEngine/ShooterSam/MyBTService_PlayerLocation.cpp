// Fill out your copyright notice in the Description page of Project Settings.


#include "MyBTService_PlayerLocation.h"
#include "Kismet/GameplayStatics.h"
#include "BehaviorTree/BlackboardComponent.h"



UMyBTService_PlayerLocation::UMyBTService_PlayerLocation()
{
	NodeName= TEXT("Get player location");
}

void UMyBTService_PlayerLocation::TickNode(UBehaviorTreeComponent& OwnerComp, uint8* NodeMemory, float DeltaSeconds)
{
	Super::TickNode(OwnerComp, NodeMemory, DeltaSeconds);
	
	APawn* PlayerPawn = UGameplayStatics::GetPlayerPawn(OwnerComp.GetWorld(), 0);

	UBlackboardComponent* Blackboard =OwnerComp.GetBlackboardComponent();
	
	if ( Blackboard && PlayerPawn)
	{
		Blackboard->SetValueAsVector(GetSelectedBlackboardKey(),PlayerPawn->GetActorLocation());
	}
}