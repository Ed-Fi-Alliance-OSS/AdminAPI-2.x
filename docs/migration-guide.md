# Migrate from previous versions

To migrate from a previous version (2.2.0) to a new version (2.2.1) you need to replace the old version binaries with the new ones.

## 1 Building the new version

### Step 1.1: Download the latest version

First, download the latest version and extract the contents of the sources directory to a directory to build the new version binaries.

### Step 1.2: Build and publish the new version binaries

Go to the root directory and execute the build and publish command to generate the new binaries.

```bash
.\build.ps1 BuildAndPublish       
```

## 2 Update AdminApi Docker Container

### Step 2.1: Pack the binaries

Go to the publish directory

```bash
cd Application\EdFi.Ods.AdminApi\publish
```

Now, pack the binaries into a tar package.

```bash
tar --exclude='appsettings*.json' --exclude='*.log' --exclude='*.sh' -cvf adminApi_publish.tar *.*
```

## Step 2.2: Identify the Docker container

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

You will need the Container Id for the adminapi contain€ to run the following commands.

## Step 2.3: Copy package to docker container

Using the container id, replace the <container-id> with the corresponding Container Id for the adminapi

```bash
docker cp adminApi_publish.tar <container-id>:/tmp/adminApi_publish.tar
```

## Step 2.4: Remove dll files from the container

To update the application you need to remove the previous dll files.  The new version has new versions of the dll files and also some packages were removed to fix vulnerabilities.

```bash
docker exec -it <container-id> sh -c "find /app -type f ! -name '*.sh' ! -name '*.config' ! -name 'appsettings*.json' -exec rm {} +"
```

## Step 2.5: Unzip the tar file into the Docker container

Now, you will need to unzip the binaries into the Docker container folder.

```bash
docker exec -it <container-id> sh -c "tar -xvf /tmp/adminApi_publish.tar -C /app/"
```

## Step 2.6: Update the appsettings file

The appsettings should be updated to add some parameters.  

### 2.6.1 Download appsettings.json
 
 First, download the appsettings.json from the Docker container to edit the file on the local computer

```bash
docker cp <container-id>:/app/appsettings.json /temp/appsettings.json
```

### 2.6.2 Edit appsettings.json file on the local computer

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

### 2.6.3 Copy the appsettings.json to the container

Copy the modified appsettings.json file back to the container

```bash
docker cp /temp/appsettings.json <container-id>:/app/appsettings.json
```

## Step 2.7: Set permissions

Now, you will need to unzip the binaries into the Docker container folder.

```bash
docker exec -u root -it <container-id> sh -c "chmod 700 /app/*"
```

```bash
 docker exec -u root -it  <container-id>  sh -c "chmod 777 /app/appsettings.json"
```

## Step 2.8 Restart the Container

To update the Docker container you need to run the following command to get the Docker Container Id.

```bash
docker restart <container-id> 
```
----------

## 3 Update AdminApi (IIS)

Open Powershell as an Admin to update the files.
Replace the source and destination vars to use your structure.

Declare the variables

```bash
$publishFolderPath = "C:\PublishFolder"
$virtualFolderPath = "C:\inetpub\wwwroot\YourVirtualFolder"
```

### Step 3.1: Remove dll files from the virtual folder

Go to the iis directory for the AdminApi site

Create a backup of the folder.

Remove all the dll files from the virtual folder

```bash
Get-ChildItem -Path $virtualFolderPath -File -Recurse | Where-Object { $_.Name -notmatch '\.sh$|\.config$|appsettings.*\.json$' } | Remove-Item
```

### Step 3.2: Copy binaries to virtual directory

```bash


Get-ChildItem -Path $publishFolderPath -File -Recurse | Where-Object { $_.Name -notmatch 'appsettings.*\.json$|\.config$' } | ForEach-Object { $destPath = $_.FullName.Replace($publishFolderPath, $virtualFolderPath); $destDir = [System.IO.Path]::GetDirectoryName($destPath); if (-not (Test-Path -Path $destDir)) { New-Item -ItemType Directory -Path $destDir -Force }; Copy-Item -Path $_.FullName -Destination $destPath }
```

### 3.3 Edit appsettings.json file

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

### 3.3 Update permissions

In some cases it is necessary to update the permissions of the binaries to be executed by the IIS.

```bash showLineNumbers
$userName = "IIS AppPool\DefaultAppPool"  # Change this to your application pool identity

# Set File System Permissions
$acl = Get-Acl $virtualFolderPath
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($userName, "ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl.SetAccessRule($accessRule)
Set-Acl $virtualFolderPath $acl
```

### 3.4 Restart IIS

To apply the changes you should restart the IIS service or the service Pool.

You can reset the IIS service. This process will affect the rest io applications.

```
iisreset
```

Or you can reset the IIS AppPool related to the site.

```
Restart-WebAppPool -Name "AdminApiAppPool"
```
