    pipeline {
        agent any
        
        environment {
            SONARQUBE = tool name: 'sonar_scanner_dotnet'
            USER_NAME = 'shubhamgoel02'
            PROJECT_ID = 'melodic-grail-321310'
            CLUSTER_NAME = 'kubernetes-cluster-shubhamgoel02'
            LOCATION = 'us-central1'
        }
        
        stages {
            stage('Clean') {
                steps {
                    cleanWs()
                }
            }
            stage('Checkout') {
                steps {
                    git branch: "${BRANCH_NAME}", url: 'https://github.com/shubhamgoel4aug/app_shubhamgoel02.git'
                }
            }
            stage('dotnet restore') {
                steps {
                    bat "dotnet restore DevOpsnMicroServices/DevOpsnMicroServices.csproj"
                }
            }
            stage('Start sonarqube analysis') {
                when {
                    branch 'master'
                }
                steps {
                    withSonarQubeEnv('Test_Sonar') {
                        bat "${SONARQUBE}\\SonarScanner.MSBuild.exe begin /k:sonar-${USER_NAME} /n:sonar-${USER_NAME} /v:1.0 /d:sonar.cs.vstest.reportsPaths=**/*.trx /d:sonar.cs.vscoveragexml.reportsPaths=**/*.coveragexml"
                    }
                }
            }
            stage('Code Build') {
                steps {                    
                    bat "dotnet build"
                }
            }
            stage('Stop sonarqube analysis') {
                when {
                        branch 'master'
                }
                steps {
                    withSonarQubeEnv('Test_Sonar') {
                        bat "${SONARQUBE}\\SonarScanner.MSBuild.exe end"
                    }
                }
            }
            stage('Docker Image') {
                steps {
                    bat 'dotnet publish DevOpsnMicroServices -o Publish -c Release'
                    bat 'docker rmi -f devopsnmicroservices:local_dev'
                    bat "docker build -f ${WORKSPACE}\\Publish\\Dockerfile -t devopsnmicroservices:local_dev ${WORKSPACE}\\Publish"
                }
            }
            stage('Containers') {
                parallel {
                    stage('PreContainerCheck') {
                        steps {
                            powershell 'if($(docker ps --filter "publish=7200" -a -q) -ne $null) {docker rm $(docker ps --filter "publish=7200" -a -q) -f}'
                            powershell 'if($(docker ps --filter "publish=7300" -a -q) -ne $null) {docker rm $(docker ps --filter "publish=7300" -a -q) -f}'
                        }                        
                    }
                    stage('PushtoDockerHub') {
                        steps {
                            withCredentials([usernamePassword(credentialsId: 'DockerHub', passwordVariable: 'DockerHubPassword', usernameVariable: 'DockerHubUserName')]) {
                                bat "docker tag devopsnmicroservices:local_dev ${USER_NAME}/i-${USER_NAME}-${BRANCH_NAME}:${BUILD_NUMBER}"
                                bat "docker tag devopsnmicroservices:local_dev ${USER_NAME}/i-${USER_NAME}-${BRANCH_NAME}:latest"
                                bat "docker login -u ${DockerHubUserName} -p ${DockerHubPassword}"
                                bat "docker push ${USER_NAME}/i-${USER_NAME}-${BRANCH_NAME}:${BUILD_NUMBER}"
                                bat "docker push ${USER_NAME}/i-${USER_NAME}-${BRANCH_NAME}:latest"
                            }
                        }                        
                    }
                }
            }
            stage('Docker Deployment') {
                parallel {
                    stage('master') {
                        when {
                            branch 'master'
                        }
                        steps {                            
                            bat "docker run -p 7200:7100 -d -e deployment.branch=master --name c-${USER_NAME}_${BRANCH_NAME} ${USER_NAME}/i-${USER_NAME}-${BRANCH_NAME}:latest"
                        }                    
                    }
                    stage('others') {
                        when {
                            not {
                                branch 'master'
                            }
                        }
                        steps {                            
                            bat "docker run -p 7300:7100 -d -e deployment.branch=${BRANCH_NAME} --name c-${USER_NAME}_${BRANCH_NAME} ${USER_NAME}/i-${USER_NAME}-${BRANCH_NAME}:latest"
                        }                    
                    }
                }
            }
            stage('Kubernetes Deployment (Local)') {
                parallel {
                    stage('master') {
                        when {
                            branch 'master'
                        }
                        steps {
                            powershell "(Get-Content ${WORKSPACE}\\deployment.yml).Replace('{{USER_NAME}}', '${USER_NAME}').Replace('{{BRANCH_NAME}}', '${BRANCH_NAME}').Replace('{{PORT}}', '30157') | Out-File ${WORKSPACE}\\deployment.master.yml"
                            bat "kubectl config use-context docker-desktop"
                            bat "kubectl apply -f ${WORKSPACE}\\deployment.master.yml"
                        }                    
                    }
                    stage('others') {
                        when {
                            not {
                                branch 'master'
                            }
                        }
                        steps {
                            powershell "(Get-Content ${WORKSPACE}\\deployment.yml).Replace('{{USER_NAME}}', '${USER_NAME}').Replace('{{BRANCH_NAME}}', '${BRANCH_NAME}').Replace('{{PORT}}', '30158') | Out-File ${WORKSPACE}\\deployment.${BRANCH_NAME}.yml"
                            bat "kubectl config use-context docker-desktop"
                            bat "kubectl apply -f ${WORKSPACE}\\deployment.${BRANCH_NAME}.yml"
                        }                    
                    }
                }
            }
            stage('Kubernetes Deployment (GKE)') {
                when {
                    branch 'master'
                }
                steps {
                    bat "kubectl config use-context gke_${PROJECT_ID}_${LOCATION}_${CLUSTER_NAME}"
                    bat "kubectl apply -f ${WORKSPACE}\\deployment.master.yml"
                }    
            }
        }
    }