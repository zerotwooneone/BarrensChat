# BarrensChat
Project to test out creating a simple chat server

Attempting to create a simple azure chat server.

## Setup
### Azure
You must create a Notification Hub in Azure and take note of the DefaultFullSharedAccessSignature located in the 'Access Policies' section of settings.
### Dev
You must add a value to user secrets with a keys of "PrimaryHub:FullAccessToken" and "PrimaryHub:Name" where values come from the above Azure Notification Hub Full-Access-Signature
