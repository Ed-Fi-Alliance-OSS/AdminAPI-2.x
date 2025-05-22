-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

-- Create Ed-Fi ODS Admin Console ClaimSet

DECLARE @claimSetName nvarchar(32)

SET @claimSetName = 'Ed-Fi ODS Admin Console'

PRINT 'Ensuring Ed-Fi ODS Admin Console Claimset exists.'

INSERT INTO EdFi_Security.dboClaimSets (ClaimSetName, IsEdfiPreset)
SELECT DISTINCT @claimSetName, 1 FROM EdFi_Security.dboClaimSets
WHERE NOT EXISTS (SELECT 1
    FROM EdFi_Security.dboClaimSets
		WHERE ClaimSetName = @claimSetName  )
GO

-- Configure Ed-Fi ODS Admin Console ClaimSet

DECLARE @actionName nvarchar(32)
DECLARE @claimSetName nvarchar(255)
DECLARE @resourceNames TABLE (ResourceName nvarchar(64))
DECLARE @resourceClaimIds TABLE (ResourceClaimId int)
DECLARE @authorizationStrategyId INT
DECLARE @ResourceClaimId INT

SET @claimSetName = 'Ed-Fi ODS Admin Console'

IF  EXISTS (SELECT 1 FROM EdFi_Security.dboClaimSets c WHERE c.ClaimSetName = @claimSetName)
BEGIN
    DECLARE @edFiOdsAdminConsoleClaimSetId as INT

    SELECT @edFiOdsAdminConsoleClaimSetId = ClaimsetId
    FROM EdFi_Security.dboClaimSets
    WHERE ClaimSets.ClaimSetName = @claimSetName

    DELETE csrcaaso
    FROM EdFi_Security.dboClaimSetResourceClaimActionAuthorizationStrategyOverrides csrcaaso
    INNER JOIN EdFi_Security.dboClaimSetResourceClaimActions ON csrcaaso.ClaimSetResourceClaimActionId = EdFi_Security.dboClaimSetResourceClaimActions.ClaimSetResourceClaimActionId
    WHERE EdFi_Security.dboClaimSetResourceClaimActions.ClaimSetId = @edFiOdsAdminConsoleClaimSetId

    DELETE FROM EdFi_Security.dboClaimSetResourceClaimActions
    WHERE ClaimSetId = @edFiOdsAdminConsoleClaimSetId

    PRINT 'Creating Temporary Records.'
    INSERT INTO @resourceNames VALUES
				('section'),
				('school'),
				('student'),
				('studentSchoolAssociation'),
				('studentSpecialEducationProgramAssociation'),
				('studentDisciplineIncidentBehaviorAssociation'),
				('studentSchoolAssociation'),
				('studentSchoolAttendanceEvent'),
				('studentSectionAssociation'),
				('staffEducationOrganizationAssignmentAssociation'),
				('staffSectionAssociation'),
				('courseTranscript')
    INSERT INTO @resourceClaimIds SELECT ResourceClaimId FROM EdFi_Security.dboResourceClaims WHERE ResourceName IN (SELECT ResourceName FROM @resourceNames)
END

SELECT @authorizationStrategyId = AuthorizationStrategyId
FROM   EdFi_Security.dboAuthorizationStrategies
WHERE  AuthorizationStrategyName = 'NoFurtherAuthorizationRequired'

DECLARE @actionId int
DECLARE @claimSetId int

SELECT @claimSetId = ClaimSetId FROM EdFi_Security.dboClaimSets WHERE ClaimSetName = @claimSetName

PRINT 'Configuring Claims for Ed-Fi ODS Admin Console Claimset...'

IF NOT EXISTS (SELECT 1
    FROM EdFi_Security.dboClaimSetResourceClaimActions csraa,EdFi_Security.dboActions a, @resourceClaimIds rc
		WHERE csraa.ActionId = a.ActionId AND ClaimSetId = @claimSetId AND csraa.ResourceClaimId = rc.ResourceClaimId)

BEGIN
    INSERT INTO EdFi_Security.dboClaimSetResourceClaimActions (ActionId, ClaimSetId, ResourceClaimId)
        SELECT ActionId, @claimSetId, rc.ResourceClaimId  
        FROM EdFi_Security.dboActions, @resourceClaimIds rc
        WHERE ActionName in ('Read')
        AND NOT EXISTS (
            SELECT 1
            FROM EdFi_Security.dboClaimSetResourceClaimActions
            WHERE ActionId = Actions.ActionId AND ClaimSetId = @claimSetId AND ResourceClaimId = rc.ResourceClaimId
        )

    INSERT INTO EdFi_Security.dboClaimSetResourceClaimActionAuthorizationStrategyOverrides (AuthorizationStrategyId, ClaimSetResourceClaimActionId)
        SELECT @authorizationStrategyId, ClaimSetResourceClaimActionId
        FROM EdFi_Security.dboClaimSetResourceClaimActions csrc
            INNER JOIN EdFi_Security.dboResourceClaims r 
                ON csrc.ResourceClaimId = r.ResourceClaimId AND csrc.ClaimSetId = @claimSetId
        WHERE r.ResourceName IN (
				    'section',
				    'school',
				    'student',
				    'studentSchoolAssociation',
				    'studentSpecialEducationProgramAssociation',
				    'studentDisciplineIncidentBehaviorAssociation',
				    'studentSchoolAssociation',
				    'studentSchoolAttendanceEvent',
				    'studentSectionAssociation',
				    'staffEducationOrganizationAssignmentAssociation',
				    'staffSectionAssociation',
				    'courseTranscript')
END 

SELECT @actionId = ActionId FROM EdFi_Security.dboActions WHERE ActionName = 'Read'
SELECT @ResourceClaimId = ResourceClaimId FROM EdFi_Security.dboResourceClaims WHERE ResourceName = 'types'

IF NOT EXISTS (
    SELECT 1 FROM EdFi_Security.dboClaimSetResourceClaimActions
		WHERE ClaimSetResourceClaimActions.ActionId = @actionId AND ClaimSetResourceClaimActions.ClaimSetId = @claimSetId
			   AND ClaimSetResourceClaimActions.ResourceClaimId = @ResourceClaimId)
BEGIN
    INSERT INTO EdFi_Security.dboClaimSetResourceClaimActions (ActionId, ClaimSetId, ResourceClaimId)
		    VALUES (@actionId, @claimSetId, @ResourceClaimId)

	  INSERT INTO EdFi_Security.dboClaimSetResourceClaimActionAuthorizationStrategyOverrides (AuthorizationStrategyId, ClaimSetResourceClaimActionId)
	      SELECT @authorizationStrategyId, ClaimSetResourceClaimActions.ClaimSetResourceClaimActionId
	      FROM EdFi_Security.dboClaimSetResourceClaimActions
	          INNER JOIN EdFi_Security.dboResourceClaims r
                ON ClaimSetResourceClaimActions.ResourceClaimId = r.ResourceClaimId
	          INNER JOIN EdFi_Security.dboActions
                ON Actions.actionId = ClaimSetResourceClaimActions.ActionId AND Actions.ActionName in ('Read')
	      WHERE r.ResourceName IN  ('types') AND ClaimSetResourceClaimActions.ActionId = @actionId AND ClaimSetResourceClaimActions.ClaimSetId = @claimSetId
END
GO
