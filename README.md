# CrowbarWebsite
This is an archive of the Web Application developed by CROWBAR for COMPX527 at The University of Waikato.

We've removed any specific AWS keys however they've also been revoked on AWS's end.

You'll need a DynamoDB database, as well as an S3 bucket to store data.

## Building
`dotnet restore && dotnet publish -c Release -p:PublishProfile=FolderProfile`

## Licensing
See LICENSE.
