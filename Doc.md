# Technical doc

## Deploying
dotnet lambda deploy-serverless conditus-trader-portfolio-service

- https://docs.aws.amazon.com/toolkit-for-visual-studio/latest/user-guide/lambda-cli-publish.html 

## Generate user tokens
aws cognito-idp initiate-auth --auth-flow USER_PASSWORD_AUTH --client-id {clientId}  --auth-parameters USERNAME={username},PASSWORD="{password}"

- https://stackoverflow.com/questions/49063292/how-to-generate-access-token-for-an-aws-cognito-user 