# Container image that runs your code
FROM mcr.microsoft.com/dotnet/sdk:6.0
RUN dotnet tool install -g dotnet-stryker

WORKDIR /app
ADD . /app

RUN export PATH="$PATH:/root/.dotnet/tools"

# RUN dotnet stryker --test-project Minor.Nijn.Test/Minor.Nijn.Test.csproj 
ENTRYPOINT ["dotnet stryker Minor.Nijn.Test/Minor.Nijn.Test.csproj"]