pipeline {
    agent any
    
    environment {
        SonarQubeTool = tool name: 'sonar_scanner_dotnet'
        Docker_Repository = 'devopsnmicroservices'
        Docker_Login_User = credentials('DockerLoginUser')
        Docker_Login_Password = credentials('DockerLoginPassword')
        UserName = 'shubhamgoel02'
    }
    
    stages {
        stage('Clean') {
            steps {
                cleanWs()
            }
        }
        stage('Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/shubhamgoel4aug/devopsnmicroservices.git'
            }
        }
        stage('Sonarqube Begin') {
            steps {
                withSonarQubeEnv('Test_Sonar') {
                    bat "${SonarQubeTool}\\SonarScanner.MSBuild.exe begin /k:sonar-${UserName} /n:sonar-${UserName} /v:1.0 /d:sonar.cs.vstest.reportsPaths=**/*.trx /d:sonar.cs.vscoveragexml.reportsPaths=**/*.coverage"
                }
            }
        }
        stage('Build') {
            steps {
                bat "dotnet restore DevOpsnMicroServices/DevOpsnMicroServices.csproj"
                bat "dotnet build"
            }
        }
        stage('Test') {
            steps {
                bat 'dotnet test --logger "trx;LogFileName=DevOpsnMicroServices.Tests.Results.trx" --no-build --collect "Code Coverage"'
                mstest testResultsFile:"**/*.trx", keepLongStdio: true
            }
        }
        stage('Sonarqube End') {
            steps {
                withSonarQubeEnv('Test_Sonar') {
                    bat "${SonarQubeTool}\\SonarScanner.MSBuild.exe end"
                }
            }
        }
        stage('Publish') {
            steps {
                bat 'dotnet publish DevOpsnMicroServices -o Publish -c Release'
                bat 'docker rmi -f devopsnmicroservices:local_dev'
                bat "docker build -f ${WORKSPACE}\\Publish\\Dockerfile -t devopsnmicroservices:local_dev ${WORKSPACE}\\Publish"
                bat "docker tag devopsnmicroservices:local_dev ${Docker_Login_User}/i-${UserName}-master:dev_${BUILD_NUMBER}"
                bat "docker login -u ${Docker_Login_User} -p ${Docker_Login_Password}"
                bat "docker push ${Docker_Login_User}/i-${UserName}-master:dev_${BUILD_NUMBER}"
            }
        }
        stage('deploy') {
            steps {
                bat 'docker rm c-${UserName}_master -f'
                bat "docker run -p 7100:7100 -d --name c-${UserName}_master ${Docker_Login_User}/i-${UserName}-master:dev_${BUILD_NUMBER}"
            }
        }
    }
}