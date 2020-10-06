cd ../database/card
docker build -t carddb .

cd ../catalog
docker build -t catalogdb .

cd ../url
docker build -t urldb .

cd ../user
docker build -t userdb .

cd ../../Src/DigitalWorkSpace/TinyUrl
docker build -t tinyurls .

cd ../User
docker build -t users .

cd ../Card
docker build -t cards .

cd ../Catalog
docker build -t catalogs .

cd ../ApiGateWay
docker build -t api .

cd ../../DigitalWorkSpaceUI
docker build -t dwsui .

