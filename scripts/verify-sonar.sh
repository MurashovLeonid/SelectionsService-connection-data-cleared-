#!/bin/bash

remove_branch() {
    SONAR_QG_RESULT=$(curl -u ${SONAR_TOKEN}: ${SONAR_URL}/api/project_branches/list?project=${CI_PROJECT_NAME} | jq --arg CI_COMMIT_REF_NAME "$CI_COMMIT_REF_NAME" '.branches[] | select(.name==$CI_COMMIT_REF_NAME).status.qualityGateStatus')
    if [ "${SONAR_QG_RESULT}" = '"OK"' ];
    then
        curl -X POST -d "project=${CI_PROJECT_NAME}&branch=${CI_COMMIT_REF_NAME}" -u ${SONAR_TOKEN}: ${SONAR_URL}/api/project_branches/list?delete=${CI_PROJECT_NAME} | jq .
        exit 0
    else
        echo "removing branch:  $CI_COMMIT_REF_NAME">&2
        exit 1
    fi
}

if [ -n "$1" ]; then
    if [ $1 = "remove_branch" ]
    then
        echo "removing branch:  $CI_COMMIT_REF_NAME">&2
        remove_branch
        exit 0
    fi
fi

set -e

echo "curl -u ${SONAR_URL}/api/project_branches/list?project=${CI_PROJECT_NAME} | jq"
curl -u ${SONAR_TOKEN}: ${SONAR_URL}/api/project_branches/list?project=${CI_PROJECT_NAME} | jq .

#Verify if a previous result exist for this branch

SONAR_PAST_RESULT=$(curl -u ${SONAR_TOKEN}: ${SONAR_URL}/api/project_branches/list?project=${CI_PROJECT_NAME} | jq --arg CI_COMMIT_REF_NAME "$CI_COMMIT_REF_NAME" '.branches[] | select(.name==$CI_COMMIT_REF_NAME).status.qualityGateStatus')

if [ -n "$SONAR_PAST_RESULT" ]
then
  echo "[INFO] Current sonar gate status is $SONAR_PAST_RESULT"
fi

#dotnet-sonarscanner begin /k:\"$sonar_project_key\" /d:sonar.host.url=\"$SONAR_URL\" /d:sonar.gitlab.project_id=\"$sonar_gitlab_project_id\" /d:sonar.login=\"$SONAR_TOKEN\" /n:\"$sonar_project_name\"  /d:sonar.gitlab.commit_sha=$CI_COMMIT_SHA /d:sonar.gitlab.ref_name=$CI_COMMIT_REF_NAME
gitlab-sonar-scanner -X -Dsonar.branch.name=${CI_COMMIT_REF_NAME} -Dsonar.branch.target=develop -Dsonar.analysis.mode=publish;

declare -i a=0

# loop until quality gate is processed by sonarqube server

while [ -z "$SONAR_QG_RESULT" ]
do

   if [ $a -ne 0 ]
   then
      sleep 25
   fi
   if [ $a -eq 10 ]
   then
      break
   fi
    curl -u ${SONAR_TOKEN}: ${SONAR_URL}/api/project_branches/list?project=${CI_PROJECT_NAME} | jq .

    SONAR_QG_RESULT=$(curl -u ${SONAR_TOKEN}: ${SONAR_URL}/api/project_branches/list?project=${CI_PROJECT_NAME} | jq --arg CI_COMMIT_REF_NAME "$CI_COMMIT_REF_NAME" '.branches[] | select(.name==$CI_COMMIT_REF_NAME).status.qualityGateStatus')
    a=$(( $a + 1 ))
    echo "SONAR GATE STATUS IS = $SONAR_QG_RESULT"
done

echo "SONAR GATE STATUS IS = $SONAR_QG_RESULT"

if [ "${SONAR_QG_RESULT}" = '"OK"' ];
then
    exit 0
else
    exit 1
fi
