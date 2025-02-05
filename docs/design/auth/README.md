# Authentication and Authorization in Admin API 2.x

## Versions 2.0, 2.1, and 2.2

### System Context 2.0 through 2.2

System administrators interact directly with Admin API to perform ODS/API
configuration tasks and manage client credentials.

```mermaid
C4Context
    Person(SysAdmin, "Platform Host Sys Admin")
    
    Enterprise_Boundary(backend, "Ed-Fi ODS/API Platform") {

        System(AdminApi, "Ed-Fi Admin API 2", "A REST API system for<br />configuration of ODS/API<br />and management of<br />client credentials")

        System(OdsApi, "Ed-Fi ODS/API", "A REST API system for<br />educational data interoperability")
    }
    
    Rel(SysAdmin, AdminApi, "Authenticates,<br />Interacts with `/v2` endpoints")
    UpdateRelStyle(SysAdmin, AdminApi, $offsetX="0", $offsetY="-30")

    Rel(AdminApi, OdsApi, "Writes admin and<br />security configuration")
    UpdateRelStyle(AdminApi, OdsApi, $offsetX="-20", $offsetY="40")
```

### Containers 2.0 through 2.2

```mermaid
C4Container
    Person(SysAdmin, "Platform Host Sys Admin")

    System_Boundary(platform, "Ed-Fi ODS/API Platform") {

        Container(AdminApi, "Ed-Fi Admin API 2")

        Container(OdsApi, "Ed-Fi ODS/API")
        
        ContainerDb(Admin, "EdFi_Admin")
        ContainerDb(Security, "EdFi_Security")
    }
    
    Rel(SysAdmin, AdminApi, "Authenticates,<br />Interacts with `/v2` endpoints")
    UpdateRelStyle(SysAdmin, AdminApi, $offsetX="0", $offsetY="-30")
    
    Rel(AdminApi, Admin, "Reads and writes")
    UpdateRelStyle(AdminApi, Admin, $offsetY="0", $offsetX="10")

    Rel(OdsApi, Admin, "Reads")
    UpdateRelStyle(OdsApi, Admin, $offsetY="20", $offsetX="-10")

    Rel(OdsApi, Security, "Reads")
    UpdateRelStyle(OdsApi, Security, $offsetY="0", $offsetX="10")

    UpdateLayoutConfig($c4ShapeInRow="2", $c4BoundaryInRow="2")
```

## Version 2.3 and above

Beginning with version 2.3, deployments will have the option of using
[Keycloak](https://www.keycloak.org/) as an external OAuth provider. This will
be _required_ when using [Admin Console](../adminconsole/readme.md). For
backwards compatibility, existing Admin API users will be able to continue
direct integrations without needing Keycloak.

### System Context 2.3+

Keycloak is an optional part of the system.

```mermaid
C4Context
    Person(SysAdmin, "Platform Host Sys Admin")
    
    Enterprise_Boundary(backend, "Backend Systems") {

        Enterprise_Boundary(platform, "Ed-Fi ODS/API Platform") {

            System(AdminApi, "Ed-Fi Admin API 2", "A REST API system for<br />configuration of ODS/API<br />and management of<br />client credentials")

            System(OdsApi, "Ed-Fi ODS/API", "A REST API system for<br />educational data interoperability")
        }
        
        Rel(SysAdmin, AdminApi, "Authenticates,<br />Interacts with `/v2` endpoints")
        UpdateRelStyle(SysAdmin, AdminApi, $offsetX="0", $offsetY="-100")

        Rel(AdminApi, OdsApi, "Writes admin and<br />security configuration")
        UpdateRelStyle(AdminApi, OdsApi, $offsetX="-30", $offsetY="40")

        System_Ext(Keycloak, "Keycloak", "An open source OpenID Connect provider")

        Rel(AdminApi, Keycloak, "OPTIONAL: Manage credentials,<br />OAuth2 token backend")
        UpdateRelStyle(AdminApi, Keycloak, $offsetX="10", $offsetY="-30")
    }
```

### System Containers 2.3+

The containers can look exactly as in Admin API 2.0 through 2.2, or alternately:

```mermaid
C4Container
    Person(SysAdmin, "Platform Host Sys Admin")


    Enterprise_Boundary(backend, "Backend Systems") {

        System_Boundary(platform, "Ed-Fi ODS/API Platform") {
            Container(AdminApi, "Ed-Fi Admin API 2")

            Container(OdsApi, "Ed-Fi ODS/API")
            
            ContainerDb(Admin, "EdFi_Admin")
            ContainerDb(Security, "EdFi_Security")
        }

        System_Boundary(keycloakBoundary, "Keycloak") {
            Container_Ext(KeycloakWeb, "Keycloak Web/API")
            ContainerDb_Ext(KeycloakDb, "Keycloak DB")

            Rel(KeycloakWeb, KeycloakDb, "Reads and<br />writes")
            UpdateRelStyle(KeycloakWeb, KeycloakDb, $offsetX="-20", $offsetY="20")
        }
        
        Rel(SysAdmin, AdminApi, "Authenticates,<br />Interacts with `/v2` endpoints")
        UpdateRelStyle(SysAdmin, AdminApi, $offsetX="-10", $offsetY="-60")
        
        Rel(AdminApi, Admin, "Reads and writes")
        UpdateRelStyle(AdminApi, Admin, $offsetY="0", $offsetX="10")

        Rel(OdsApi, Admin, "Reads")
        UpdateRelStyle(OdsApi, Admin, $offsetY="20", $offsetX="-10")

        Rel(OdsApi, Security, "Reads")
        UpdateRelStyle(OdsApi, Security, $offsetY="0", $offsetX="10")

        Rel(AdminApi, KeycloakWeb, "Manage credentials, OAuth2 token backend")
        UpdateRelStyle(AdminApi, KeycloakWeb, $offsetX="10", $offsetY="30")
    }

    UpdateLayoutConfig($c4ShapeInRow="2", $c4BoundaryInRow="1")
```

## Solution Design

* [Self-Contained Authentication with OpenIdDict](./SELF-CONTAINED.md)
* [Keycloak Integration](./KEYCLOAK.md)
