/*
Post-Deployment Script Template                            
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.        
 Use SQLCMD syntax to include a file in the post-deployment script.            
 Example:      :r .\myfile.sql                                
 Use SQLCMD syntax to reference a variable in the post-deployment script.        
 Example:      :setvar TableName MyTable                            
               SELECT * FROM [$(TableName)]                    
--------------------------------------------------------------------------------------
*/


:r .\Script\PostDeploymentScript\PopulateCapability.sql
:r .\Script\PostDeploymentScript\PopulateCapabilityFilter.sql
:r .\Script\PostDeploymentScript\PopulateRole.sql
:r .\Script\PostDeploymentScript\PopulateApplicationConfiguration.sql
:r .\Script\PostDeploymentScript\PopulateSensorType.sql
:r .\Script\PostDeploymentScript\SetSensorTypeCapabilities.sql