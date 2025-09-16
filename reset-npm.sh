#!/bin/bash
start=$PWD

echo -n "Removing all node_modules for PortalApp..." 
cd $start
cd ./PortalApp/ClientApp && rm -rf node_modules
echo "done"

echo -n "Removing all node_modules for DesignerApp..." 
cd $start
cd ./DesignerApp/TaskPane && rm -rf node_modules
cd $start
echo "done"

echo "Running npm install for PortalApp"
cd $start
cd ./PortalApp/ClientApp && npm install
cd $start
echo "done"

echo "Running npm install for DesignerApp"
cd $start
cd ./DesignerApp/TaskPane && npm install
cd $start
echo "done"