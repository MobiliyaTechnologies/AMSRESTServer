# Asset Monitoring REST API Server


## Getting Started

### Introduction

It contains following projects
1. RestService
	It contain back end API's. For API's help page redirect to http://$(Hosted URL)/Help.
	For offline  documentation, download 'AssetMonitoringApi.zip' file and load index.html in browser.

### Prerequisites

What things you need to install the software and how to install them

* Active Azure subscription.
* Microsoft SQL Server.						
* B2C Application with sign-up, sign-in and change password policy.
* Azure storage.
* Add value for following configurations,
	* RestService Configuration - 	
        * b2c:Tenant - B2C application id.
        * b2c:ClientId - Valid client id, API only accepts tokens from its own clients.
        * b2c:SignUpPolicyId - B2c sign-in policy.
        * b2c:SignInPolicyId - B2c sign-up policy.
        * b2c:ChangePasswordPolicy - B2c change password policy.
        * b2c:ClientSecret - B2c app key.
        * BlobStorageConnectionString - azure storage connection string.

### Installing

A step by step series of examples that tell you have to get a development env running

* Clone or download source code.
* Open code in microsoft visual studio and rebuild it.
* Create and publish database.
* Host Api on IIS.
