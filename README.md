# sigma
Exercise project for Sigma IOT

Program is a ASP .Net Core 3.0 API project.
A background service fetches data from the blob and stores the data in memory.
By using polymorphism it is possible to add new sensor types to the project and encapsulating each sensor type with its own special functionality such as for example converting and returning temperature as fahrenheit.
https://sigma-exercise.azurewebsites.net/sensordata/fordevice/?deviceId=dockan&sensorType=temperature&startDate=2019-01-10
This endpoint will automatically convert temperature data to fahrenheit. This is just to demonstrate the idea of polymorphism.

A better way is to store the data in elasticsearch by using the NEST client to write and read data.
https://sigma-exercise.azurewebsites.net/elasticsearch/fordevice/?deviceId=dockan&sensorType=temperature&startDate=2019-01-10

A better architecture would be to have a separate service parse data fron the csv-files and insert the data into ES every night.
A second service would act as a API that reads from ES.
