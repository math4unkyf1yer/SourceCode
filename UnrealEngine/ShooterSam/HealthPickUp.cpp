// Fill out your copyright notice in the Description page of Project Settings.


#include "HealthPickUp.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"


// Sets default values
AHealthPickUp::AHealthPickUp()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
	
	BoxComp = CreateDefaultSubobject<UBoxComponent>(FName("BoxComp"));
	RootComponent = BoxComp;
	

	BoxComp->OnComponentBeginOverlap.AddDynamic(this, &AHealthPickUp::OnBoxBeginOverlap);
	
	StaticMeshComp = CreateDefaultSubobject<UStaticMeshComponent>(FName("StaticMesh"));
	StaticMeshComp->SetupAttachment(RootComponent);
	
	StaticMeshComp->SetCollisionEnabled(ECollisionEnabled::NoCollision);
}

// Called when the game starts or when spawned
void AHealthPickUp::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void AHealthPickUp::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

void AHealthPickUp::OnBoxBeginOverlap(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	if (OtherActor && OtherActor != this)
	{
		AShooterSamCharacter* HitCharacter = Cast<AShooterSamCharacter>(OtherActor);
		if (HitCharacter)
		{
			if (HitCharacter->CanHeal())
			{
				HitCharacter->IncreaseHealth(HealAmount);	
				Destroy();
			}else
			{
				UE_LOG(LogTemp,Warning,TEXT("to much health"));	
			}
		}
	}
}

void AHealthPickUp::HealPlayer(float Amount, AActor* OtherActor)
{
	
}



