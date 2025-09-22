-- Add Workflow Fields to Incidents Table
-- Date: 2025-09-19
-- Description: Add new workflow fields for 6-stage workflow

USE LTMDB;
GO

-- Add new workflow fields
ALTER TABLE Incidents ADD
    WorkflowStatus INT NULL,
    DueDate DATETIME2 NULL,
    ResponseStartDate DATETIME2 NULL,
    CauseAnalysisDate DATETIME2 NULL,
    CompletionDate DATETIME2 NULL,
    PreventionProposalDate DATETIME2 NULL,
    EffectivenessConfirmationDate DATETIME2 NULL,
    ResponseContent NVARCHAR(2000) NULL;

-- Add CHECK constraint for WorkflowStatus
ALTER TABLE Incidents 
ADD CONSTRAINT CK_Incidents_WorkflowStatus 
CHECK (WorkflowStatus IS NULL OR WorkflowStatus BETWEEN 1 AND 6);

-- Add indexes for performance
CREATE NONCLUSTERED INDEX IX_Incidents_WorkflowStatus 
ON Incidents (WorkflowStatus) 
WHERE WorkflowStatus IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_Incidents_DueDate 
ON Incidents (DueDate) 
WHERE DueDate IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_Incidents_ResponseStartDate 
ON Incidents (ResponseStartDate) 
WHERE ResponseStartDate IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_Incidents_CompletionDate 
ON Incidents (CompletionDate) 
WHERE CompletionDate IS NOT NULL;

-- Verify changes
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Incidents' 
AND COLUMN_NAME IN (
    'WorkflowStatus',
    'DueDate',
    'ResponseStartDate',
    'CauseAnalysisDate',
    'CompletionDate',
    'PreventionProposalDate',
    'EffectivenessConfirmationDate',
    'ResponseContent'
)
ORDER BY ORDINAL_POSITION;

PRINT 'Workflow fields added successfully';

