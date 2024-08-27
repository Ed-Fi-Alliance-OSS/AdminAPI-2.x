# Migrate from previous versions (Docker container)

You can find the preparation steps to publish the binaries in the following [Migration Guide](./migration-guide.md)

## Update AdminApi Docker Container

### Step 1: Pack the binaries

Go to the publish directory

```bash
cd Application\EdFi.Ods.AdminApi\publish
```

Now, pack the binaries into a tar package.

```bash
tar --exclude='appsettings*.json' --exclude='*.log' --exclude='*.sh' -cvf adminApi_publish.tar *.*
```

## Step 2: Identify the Docker container

To update the Docker container you need to run the following command to get the Docker Container Id.

```bash
docker ps --format '{{.ID}} {{.Image}} {{.Names}}'
```

The result of this command will be like the following

| CONTAINER ID | IMAGE | NAMES |
| -- | -- | -- |
| 91d478e194d7 | singletenant-adminapi | adminapi-packaged
| 35afe7e06bdc | singletenant-nginx | ed-fi-gateway-adminapi-packaged |
| 81c223f544f7 | singletenant-db-admin | ed-fi-db-admin-adminapi

You will need the Container Id for the adminapi container to run the following commands.

## Step 3: Copy package to docker container

Using the container id, replace the <container-id> with the corresponding Container Id for the adminapi

```bash
docker cp adminApi_publish.tar <container-id>:/tmp/adminApi_publish.tar
```

## Step 4: Remove dll files from the container

To update the application you need to remove the previous dll files.  The new version has new versions of the dll files and also some packages were removed to fix vulnerabilities.

```bash
docker exec -it <container-id> sh -c "find /app -type f ! -name '*.sh' ! -name '*.config' ! -name 'appsettings*.json' -exec rm {} +"
```

## Step 5: Unzip the tar file into the Docker container

Now, you will need to unzip the binaries into the Docker container folder.

```bash
docker exec -it <container-id> sh -c "tar -xvf /tmp/adminApi_publish.tar -C /app/"
```

## Step 6: Update the appsettings file

The appsettings should be updated to add some parameters.  

### 6.1 Download appsettings.json
 
 First, download the appsettings.json from the Docker container to edit the file on the local computer

```bash
docker cp <container-id>:/app/appsettings.json /temp/appsettings.json
```

### 6.2 Edit appsettings.json file on the local computer

Using a text editor add the following lines.

For the AppSettings section add the parameter

```
"PreventDuplicateApplications": "false"
```

After the AllowedHosts parameter, add the following  section

```
"IpRateLimiting": {
        "EnableEndpointRateLimiting": true,
        "StackBlockedRequests": false,
        "RealIpHeader": "X-Real-IP",
        "ClientIdHeader": "X-ClientId",
        "HttpStatusCode": 429,
        "IpWhitelist": [],
        "EndpointWhitelist": [],
        "GeneralRules": [
            {
                "Endpoint": "POST:/Connect/Register",
                "Period": "1m",
                "Limit": 3
            }
        ]
    }
```

### 6.3 Copy the appsettings.json to the container

Copy the modified appsettings.json file back to the container

```bash
docker cp /temp/appsettings.json <container-id>:/app/appsettings.json
```

## Step 7: Set permissions

Now, you will need to unzip the binaries into the Docker container folder.

```bash
docker exec -u root -it <container-id> sh -c "chmod 700 /app/*"
```

```bash
 docker exec -u root -it  <container-id>  sh -c "chmod 777 /app/appsettings.json"
```

## Step 8 Restart the Container

To update the Docker container you need to run the following command to get the Docker Container Id.

```bash
docker restart <container-id> 
```
----------
