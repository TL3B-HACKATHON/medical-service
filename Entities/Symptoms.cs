﻿namespace medical_profile_service.Entities;

public record SymptomsAnalysis(
    string Id,
    string Date,
    string DoctorId,
    string[] Symptoms
);
public record SymptomsAnalysisDto(
    string DoctorId,
    string Date,
    string[] Symptoms
)
{
    public SymptomsAnalysis ToSymptomsAnalysis()
        => new SymptomsAnalysis(Guid.NewGuid().ToString(), Date, DoctorId, Symptoms);
}
public record Symptoms(
    bool Itching,
    bool SkinRash,
    bool NodalSkinEruptions,
    bool ContinuousSneezing,
    bool Shivering,
    bool Chills,
    bool JointPain,
    bool StomachPain,
    bool Acidity,
    bool UlcersOnTongue,
    bool MuscleWasting,
    bool Vomiting,
    bool BurningMicturition,
    bool SpottingUrination,
    bool Fatigue,
    bool WeightGain,
    bool Anxiety,
    bool ColdHandsAndFeets,
    bool MoodSwings,
    bool WeightLoss,
    bool Restlessness,
    bool Lethargy,
    bool PatchesInThroat,
    bool IrregularSugarLevel,
    bool Cough,
    bool HighFever,
    bool SunkenEyes,
    bool Breathlessness,
    bool Sweating,
    bool Dehydration,
    bool Indigestion,
    bool Headache,
    bool YellowishSkin,
    bool DarkUrine,
    bool Nausea,
    bool LossOfAppetite,
    bool PainBehindTheEyes,
    bool BackPain,
    bool Constipation,
    bool AbdominalPain,
    bool Diarrhoea,
    bool MildFever,
    bool YellowUrine,
    bool YellowingOfEyes,
    bool AcuteLiverFailure,
    bool FluidOverload,
    bool SwellingOfStomach,
    bool SwelledLymphNodes,
    bool Malaise,
    bool BlurredAndDistortedVision,
    bool Phlegm,
    bool ThroatIrritation,
    bool RednessOfEyes,
    bool SinusPressure,
    bool RunnyNose,
    bool Congestion,
    bool ChestPain,
    bool WeaknessInLimbs,
    bool FastHeartRate,
    bool PainDuringBowelMovements,
    bool PainInAnalRegion,
    bool BloodyStool,
    bool IrritationInAnus,
    bool NeckPain,
    bool Dizziness,
    bool Cramps,
    bool Bruising,
    bool Obesity,
    bool SwollenLegs,
    bool SwollenBloodVessels,
    bool PuffyFaceAndEyes,
    bool EnlargedThyroid,
    bool BrittleNails,
    bool SwollenExtremeties,
    bool ExcessiveHunger,
    bool ExtraMaritalContacts,
    bool DryingAndTinglingLips,
    bool SlurredSpeech,
    bool KneePain,
    bool HipJointPain,
    bool MuscleWeakness,
    bool StiffNeck,
    bool SwellingJoints,
    bool MovementStiffness,
    bool SpinningMovements,
    bool LossOfBalance,
    bool Unsteadiness,
    bool WeaknessOfOneBodySide,
    bool LossOfSmell,
    bool BladderDiscomfort,
    bool FoulSmellOfurine,
    bool ContinuousFeelOfUrine,
    bool PassageOfGases,
    bool InternalItching,
    bool ToxicLookTyphos,
    bool Depression,
    bool Irritability,
    bool MusclePain,
    bool AlteredSensorium,
    bool RedSpotsOverBody,
    bool BellyPain,
    bool AbnormalMenstruation,
    bool DischromicPatches,
    bool WateringFromEyes,
    bool IncreasedAppetite,
    bool Polyuria,
    bool FamilyHistory,
    bool MucoidSputum,
    bool RustySputum,
    bool LackOfConcentration,
    bool VisualDisturbances,
    bool ReceivingBloodTransfusion,
    bool ReceivingUnsterileInjections,
    bool Coma,
    bool StomachBleeding,
    bool DistentionOfAbdomen,
    bool HistoryOfAlcoholConsumption,
    bool FluidOverload1,
    bool BloodInSputum,
    bool ProminentVeinsOnCalf,
    bool Palpitations,
    bool PainfulWalking,
    bool PusFilledPimples,
    bool Blackheads,
    bool Scurring,
    bool SkinPeeling,
    bool SilverLikeDusting,
    bool SmallDentsInNails,
    bool InflammatoryNails,
    bool Blister,
    bool RedSoreAroundNose,
    bool YellowCrustOoze
);