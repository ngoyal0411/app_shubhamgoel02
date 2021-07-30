    pipeline {
        agent any
        
        environment {
            SonarQubeTool = tool name: 'sonar_scanner_dotnet'
            UserName = 'shubhamgoel02'
            GKEProjectId = 'melodic-grail-321310'
            GKEClusterName = 'kubernetes-cluster-shubhamgoel02'
            GKELocation = 'us-central1'
            GKEDeploymentFile = 'deployment.yml'
            GKECredentials = 'GKEK8sKey'
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
            stage('Start sonarqube analysis') {
                when {
                    branch 'main'
                }
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
            stage('Unit Testing') {
                steps {
                    bat 'dotnet test --logger "trx;LogFileName=DevOpsnMicroServices.Tests.Results.trx" --no-build --collect "Code Coverage"'
                    mstest testResultsFile:"**/*.trx", keepLongStdio: true
                }
            }
            stage('Stop sonarqube analysis') {
                when {
                        branch 'main'
                }
                steps {
                    withSonarQubeEnv('Test_Sonar') {
                        bat "${SonarQubeTool}\\SonarScanner.MSBuild.exe end"
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
                                bat "docker tag devopsnmicroservices:local_dev ${UserName}/i-${UserName}-${BRANCH_NAME}:${BUILD_NUMBER}"
                                bat "docker tag devopsnmicroservices:local_dev ${UserName}/i-${UserName}-${BRANCH_NAME}:latest"
                                bat "docker login -u ${DockerHubUserName} -p ${DockerHubPassword}"
                                bat "docker push ${UserName}/i-${UserName}-${BRANCH_NAME}:${BUILD_NUMBER}"
                                bat "docker push ${UserName}/i-${UserName}-${BRANCH_NAME}:latest"
                            }
                        }                        
                    }
                }
            }
            stage('Docker Deployment') {
                parallel {
                    stage('main') {
                        when {
                            branch 'main'
                        }
                        steps {                            
                            bat "docker run -p 7200:7100 -d -e deployment.branch=main --name c-${UserName}_${BRANCH_NAME} ${UserName}/i-${UserName}-${BRANCH_NAME}:latest"
                        }                    
                    }
                    stage('others') {
                        when {
                            not {
                                branch 'main'
                            }
                        }
                        steps {                            
                            bat "docker run -p 7300:7100 -d -e deployment.branch=${BRANCH_NAME} --name c-${UserName}_${BRANCH_NAME} ${UserName}/i-${UserName}-${BRANCH_NAME}:latest"
                        }                    
                    }
                }
            }
            stage('Kubernetes Deployment (local)') {
                parallel {
                    stage('main') {
                        when {
                            branch 'main'
                        }
                        steps {
                            powershell "(Get-Content ${WORKSPACE}\\deployment.yml).Replace('{{USERNAME}}', '${UserName}').Replace('{{BRANCH_NAME}}', '${BRANCH_NAME}').Replace('{{BUILD_NUMBER}}', '${BUILD_NUMBER}').Replace('{{PORT}}', '30157') | Out-File ${WORKSPACE}\\deployment.yml"
                            bat "kubectl apply -f ${WORKSPACE}\\deployment.yml"
                        }                    
                    }
                    stage('others') {
                        when {
                            not {
                                branch 'main'
                            }
                        }
                        steps {
                            powershell "(Get-Content ${WORKSPACE}\\deployment.yml).Replace('{{USERNAME}}', '${UserName}').Replace('{{BRANCH_NAME}}', '${BRANCH_NAME}').Replace('{{BUILD_NUMBER}}', '${BUILD_NUMBER}').Replace('{{PORT}}', '30158') | Out-File ${WORKSPACE}\\deployment.yml"
                            bat "kubectl apply -f ${WORKSPACE}\\deployment.yml"
                        }                    
                    }
                }
            }
            stage('Kubernetes Deployment (GKE)') {
                when {
                    branch 'develop'
                }
                steps {
                    step([$class: 'KubernetesEngineBuilder', projectId: env.GKEProjectId, clusterName: env.GKEClusterName, location: env.GKELocation, manifestPattern: env.GKEDeploymentFile, credentialsId: env.GKECredentials, verifyDeployments: true])
                }
            }
        }
    }