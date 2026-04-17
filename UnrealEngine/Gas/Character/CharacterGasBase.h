// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "AbilitySystemInterface.h"
#include "AbilitySystemComponent.h"
#include "CharacterGasBase.generated.h"

UCLASS()
class GASLEARNING_API ACharacterGasBase : public ACharacter, public IAbilitySystemInterface
{
	GENERATED_BODY()

public:
	// Sets default values for this character's properties
	ACharacterGasBase();
	
	//Ability system component
	UPROPERTY(VisibleAnywhere,BlueprintReadOnly,Category="Abilities")
	class UAbilitySystemComponent* AbilitySystemComponent;
	
	UPROPERTY(VisibleAnywhere,BlueprintReadOnly,Category="Abilities")
	class UBasicAttributeSet* BasicAtSet;

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	
	virtual void PossessedBy(AController* NewController) override;//enemy
	
	virtual void OnRep_PlayerState() override;//player replicated
	
	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="Abilities")
	EGameplayEffectReplicationMode ReplicationMode = EGameplayEffectReplicationMode::Mixed;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

	
	virtual  UAbilitySystemComponent* GetAbilitySystemComponent() const override;
};
