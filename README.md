# sigma
Problem to solve:
Sensor data is stored in a blob storage.
Fetch the sensor data and store in a datastore with better structure.
Build an API to fetch the data based on sensor type and date.

Solution is built around an ASP .Net Core 3.0 API project.
On startup the latest data is fetched and then stored in Elasticsearch.

Technologies used:
Swagger for documenting the API's directly from the endpoint.
CQRS for lean controller.
Elasticsearch is used for logging events.

Improvements:
A better architecture would be to have a separate service parse data fron the csv-files in the blob storage and 
insert the data into ES every night. This is a work in progress almost done.
A second service would act as a API that reads from ES. 
Unit tests.

For testing Locally
Run "docker-compose up" to start ElasticSearch/Kibana.

To access the in-memory sensor data API:
http://localhost:5000/api/v1/sensordata/fordevice/?deviceId=dockan&sensorType=temperature&startDate=2019-01-10

Kibana:
http://localhost:5601/

Swagger endpoint:
http://localhost:5000/swagger/index.html
