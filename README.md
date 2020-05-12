# sigma
Exercise project for Sigma IoT

Problem to solve:
Sensor data is stored in a blob storage.
Build an API to fetch the data based on sensor type and date.
There are three sensor types with time stamps.

Solution is built around a ASP .Net Core 3.0 API project.
A background service fetches data from the blob and stores the data in-memory. Each sensor is represented by its own class that inherits from an abstract base class.
By using polymorphism it is possible to add new sensor types to the project and encapsulating functionality such as for example converting and returning temperature as fahrenheit.
https://sigma-exercise.azurewebsites.net/sensordata/fordevice/?deviceId=dockan&sensorType=temperature&startDate=2019-01-10
This endpoint will automatically convert temperature data to fahrenheit. This is just to demonstrate the idea of polymorphism.

A better way is to let the background servie fetch the data and store the data in elasticsearch by using the NEST-client to write and read data.
This endpoint uses ES to fetch data.
https://sigma-exercise.azurewebsites.net/elasticsearch/fordevice/?deviceId=dockan&sensorType=temperature&startDate=2019-01-10

Link to Kibana to visualize the data that is stored in ES.
https://2bbb9edde2bf4db3b8eac26acd9185bd.westeurope.azure.elastic-cloud.com:9243/app/kibana#/discover?_g=(refreshInterval:(pause:!t,value:0),time:(from:'2019-01-09T23:00:00.000Z',to:'2019-01-18T11:00:00.000Z'))&_a=(columns:!(SensorType,DeviceID),index:ea107b90-0305-11ea-ab7c-254d8255474d,interval:auto,query:(language:kuery,query:''),sort:!(!(MeasurementDay,desc)))

Other improvements to the project:
Swagger for documenting the API's directly from the endpoint.
Make the controller thinner by implementing CQRS with mediator for encapsulating the business logic. 
A better architecture would be to have a separate service parse data fron the csv-files in the blob storage and 
insert the data into ES every night.
A second service would act as a API that reads from ES. Unit tests.

For testing Locally
Run "docker-compose up" to start ElasticSearch/Kibana.
Kibana:
http://localhost:5601/

To access the in-memory sensor data API:
http://localhost:5000/api/v1/sensordata/fordevice/?deviceId=dockan&sensorType=temperature&startDate=2019-01-10

Swagger endpoint:
http://localhost:5000/swagger/index.html
