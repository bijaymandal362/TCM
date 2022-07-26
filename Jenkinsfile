pipeline {
    agent any
    tools {nodejs "nodejs-16"}
    environment {
        REGISTRY = 'registry.ekbana.net'
        HARBOR_NAMESPACE = "tcmv2"
        HARBOR_CREDENTIAL = credentials('harbor-login-cred')
        APP_NAME = "tcm-api"
        DIR_NAME = getDirName(env.BRANCH_NAME)
        scannerHome = tool 'SonarMSBuild';
    }
    stages {
        stage('get_commit_msg') {
            steps {
              script {
                notifyStarted()
                passedBuilds = []
                lastSuccessfulBuild(passedBuilds, currentBuild);
                env.changeLog = getChangeLog(passedBuilds)
                echo "changeLog \n${env.changeLog}"
              }
            }
        }
        stage("Checkout code") {
            steps {
                checkout scm
            }
        }
        stage('Analysis & Deploy') {     
        parallel{
          stage('SonarQube Analysis') {            
            steps {           
              withSonarQubeEnv(installationName: 'SonarQubePro') {
              sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll begin /k:\"TCMSystem_TestCaseManagementV2_AX2PL0DKAug45V0lOzvv\""
              sh "dotnet build TCM.sln"
              sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll end"
            }
            }
          }
          stage('Build & Deploy'){        
            stages{
              stage("Build image") {
                  steps {
                      echo 'test'
                      sh 'docker build -t $REGISTRY/$HARBOR_NAMESPACE/$APP_NAME:SNAPSHOT-$BRANCH_NAME-$BUILD_NUMBER .'
                  }
              }
              stage("Harbor login & Push image") {
                steps {
                    
                    sh '''echo $HARBOR_CREDENTIAL_PSW | docker login $REGISTRY -u 'harbor-user' --password-stdin'''
                    sh 'docker push  $REGISTRY/$HARBOR_NAMESPACE/$APP_NAME:SNAPSHOT-$BRANCH_NAME-$BUILD_NUMBER'
                    sh 'docker rmi $REGISTRY/$HARBOR_NAMESPACE/$APP_NAME:SNAPSHOT-$BRANCH_NAME-$BUILD_NUMBER'
                }
              }
              stage('Deploy to server') {
                steps{
                  script {
                    withKubeConfig([credentialsId: 'tcm-k8s-login', serverUrl: 'https://36841342-00f9-4c7c-8ae4-0683ebc281f6.k8s.ondigitalocean.com']) {
                    //sh 'kubectl config set current-context do-sgp1-k8s-tcm-stag'
                    //sh 'kubectl config get-contexts'
                    sh 'kubectl apply -f $DIR_NAME/api/'
                    sh 'kubectl set image deployment/$BRANCH_NAME-tcm-api-deployment $BRANCH_NAME-tcm-api=registry.ekbana.net/tcmv2/tcm-api:SNAPSHOT-$BRANCH_NAME-$BUILD_NUMBER'
                    }
                  }
                }
              }
            }  
          }
        }
        }        
    }    
    
    

    post{
      success{
        //script {
          //if (env.BRANCH_NAME == 'dev' || env.BRANCH_NAME == 'QA' || env.BRANCH_NAME == 'UAT' )
            notifySuccessful()
        //}
      }
      failure{
        notifyFailed()
      }
    }
}

def notifyStarted() {
mattermostSend (
  color: "#2A42EE",
  channel: 'tcm-jenkins',
  endpoint: 'https://ekbana.letsperk.com/hooks/kfjzs4qxhjyizdam9f5feigpre',
  message: "Build STARTED: ${env.JOB_NAME} #${env.BUILD_NUMBER} (<${env.BUILD_URL}|Link to build>)"
  )
}


def notifySuccessful() {
mattermostSend (
  color: "#00f514",
  channel: 'tcm-jenkins',
  endpoint: 'https://ekbana.letsperk.com/hooks/kfjzs4qxhjyizdam9f5feigpre',
  message: "Build SUCCESS: ${env.JOB_NAME} #${env.BUILD_NUMBER} (<${env.BUILD_URL}|Link to build>):\n${changeLog}"
  )
}

def notifyFailed() {
mattermostSend (
  color: "#e00707",
  channel: 'tcm-jenkins',
  endpoint: 'https://ekbana.letsperk.com/hooks/kfjzs4qxhjyizdam9f5feigpre',
  message: "Build FAILED: ${env.JOB_NAME} #${env.BUILD_NUMBER} (<${env.BUILD_URL}|Link to build>)"
  )
}
def lastSuccessfulBuild(passedBuilds, build) {
  if ((build != null) && (build.result != 'SUCCESS')) {
      passedBuilds.add(build)
      lastSuccessfulBuild(passedBuilds, build.getPreviousBuild())
   }
}

@NonCPS
def getChangeLog(passedBuilds) {
    def log = ""
    for (int x = 0; x < passedBuilds.size(); x++) {
        def currentBuild = passedBuilds[x];
        def changeLogSets = currentBuild.changeSets
        for (int i = 0; i < changeLogSets.size(); i++) {
            def entries = changeLogSets[i].items
            for (int j = 0; j < entries.length; j++) {
                def entry = entries[j]
                log += "* ${entry.msg} by ${entry.author} \n"
            }
        }
    }
    return log;
  }

//Getting branch name for respective branches
def getDirName(branchName) {
    if("dev".equals(branchName)) {
        return "/var/lib/jenkins/workspace/tcm-k8s/dev";
    } else if ("qa".equals(branchName)) {
        return "/var/lib/jenkins/workspace/tcm-k8s/qa";
    } else if ("uat".equals(branchName)) {
        return "/var/lib/jenkins/workspace/tcm-k8s/uat";
    } else {
        return "/var/lib/jenkins/workspace/tcm-k8s/master";
    }
}
