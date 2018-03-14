#!/usr/bin/env bash

# Definicion de variables
declare -a arr=("azurebrains.storage.webspa" 
                "azurebrains.valetkey.webapi")

for i in "${arr[@]}"
do     
    container=$(docker ps -a -q --filter ancestor=$i:latest)
    if [[ ! $container ]]; then
        # no hay ningun contenedor ejecutandose
        run=$(docker exec -it -d $container dotnet $i &)
    else
        # el contenedor estaba ejecutandose
        exec=$(docker exec -it -d $container dotnet $i &)
        echo "docker exec -it -d $container dotnet $i &"
    fi
done