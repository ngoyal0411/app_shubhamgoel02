pipeline {
    agent any
    
    environment {
        Sonarqube_Key = credentials('Sonarqube')
        Docker_Repository = 'devopsnmicroservices'
        Docker_Login_User = credentials('DockerLoginUser')
        Docker_Login_Password = credentials('DockerLoginPassword')
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
                bat "dotnet sonarscanner begin /k:DevOpsnMicroServices /d:sonar.login=${Sonarqube_Key} /d:sonar.host.url=http://localhost:9000"
                
            }
        }
        stage('Build') {
            steps {
                bat "dotnet restore DevOpsnMicroServices/DevOpsnMicroServices.csproj"
                bat "dotnet build"
            }
        }
        stage('Sonarqube End') {
            steps {
                bat "dotnet sonarscanner end /d:sonar.login=${Sonarqube_Key}"
            }
        }
        stage('Test') {
            steps {
                bat 'dotnet test --logger "trx;LogFileName=DevOpsnMicroServices.Tests.Results.trx" --no-build --collect "Code Coverage"'
                mstest testResultsFile:"**/*.trx", keepLongStdio: true
            }
        }
        stage('Publish') {
            steps {
                bat 'dotnet publish DevOpsnMicroServices -o Publish -c Release'
                bat 'docker rmi -f devopsnmicroservices:local_dev'
                bat "docker build -f ${WORKSPACE}\\Publish\\Dockerfile -t devopsnmicroservices:local_dev ${WORKSPACE}\\Publish"
                bat "docker tag devopsnmicroservices:local_dev ${Docker_Login_User}/${Docker_Repository}:dev_${BUILD_NUMBER}"
                bat "docker login -u ${Docker_Login_User} -p ${Docker_Login_Password}"
                bat "docker push ${Docker_Login_User}/${Docker_Repository}:dev_${BUILD_NUMBER}"
            }
        }
        stage('deploy') {
            steps {
                bat 'docker rm devopsnmicroservices_container -f'
                bat "docker run -p 7100:7100 -d --name devopsnmicroservices_container ${Docker_Login_User}/${Docker_Repository}:dev_${BUILD_NUMBER}"
            }
        }
    }
}