terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }

  backend "azurerm" {
    # Backend configuration is provided via CLI during terraform init
    # See GitHub Actions workflow for configuration details
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "pixelbadger" {
  name     = "rg-${var.workload}-${var.environment}"
  location = var.location

  tags = local.tags
}

resource "azurerm_service_plan" "pixelbadger" {
  name                = "asp-${var.workload}-${var.environment}"
  resource_group_name = azurerm_resource_group.pixelbadger.name
  location            = azurerm_resource_group.pixelbadger.location
  os_type             = "Linux"
  sku_name            = "F1"

  tags = local.tags
}

resource "azurerm_linux_web_app" "pixelbadger" {
  name                = "app-${var.workload}-${var.environment}"
  resource_group_name = azurerm_resource_group.pixelbadger.name
  location            = azurerm_service_plan.pixelbadger.location
  service_plan_id     = azurerm_service_plan.pixelbadger.id

  site_config {
    always_on = false

    application_stack {
      dotnet_version = "8.0"
    }
  }

  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE" = "1"
    "ASPNETCORE_ENVIRONMENT"   = var.environment
  }

  tags = local.tags
}