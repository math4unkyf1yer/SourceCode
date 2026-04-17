// Fill out your copyright notice in the Description page of Project Settings.


#include "ShooterAI.h"
#include "BehaviorTree/BlackboardComponent.h"
#include "Kismet/GameplayStatics.h"


void AShooterAI::BeginPlay()
{
	Super::BeginPlay();
	//begin play
	PlayerPawn = UGameplayStatics::GetPlayerPawn(GetWorld(),0);
	
}

// Called every frame
void AShooterAI::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	
	/*//move to actor
	if (PlayerPawn)
	{
		if (LineOfSightTo(PlayerPawn))
		{
			SetFocus(PlayerPawn);//focus on pawn
			//MoveToActor(PlayerPawn,400);	
		}else
		{
			ClearFocus(EAIFocusPriority::Gameplay);
			
			//StopMovement();
		}
	}*/
}

void AShooterAI::StartBehaviorTree(AShooterSamCharacter* Player)
{
	if (EnemyAIBehaviorTree)
	{
		MyCharacter = Cast<AShooterSamCharacter>(GetPawn());
		
		if (Player)
		{
			PlayerCharacter = Player;
		}
		
		RunBehaviorTree(EnemyAIBehaviorTree);
		
		UBlackboardComponent* EnemyBlackBoard = GetBlackboardComponent();
		
		if (EnemyBlackBoard && PlayerCharacter && MyCharacter)
		{
			EnemyBlackBoard->SetValueAsVector("EnemyStartLocation",MyCharacter->GetActorLocation());
		}
	}
}
