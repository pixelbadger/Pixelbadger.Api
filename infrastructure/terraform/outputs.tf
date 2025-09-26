output "resource_group_name" {
  description = "The name of the resource group"
  value       = azurerm_resource_group.pixelbadger.name
}

output "app_service_plan_name" {
  description = "The name of the App Service Plan"
  value       = azurerm_service_plan.pixelbadger.name
}

output "app_service_name" {
  description = "The name of the App Service"
  value       = azurerm_linux_web_app.pixelbadger.name
}

output "app_service_url" {
  description = "The URL of the deployed App Service"
  value       = "https://${azurerm_linux_web_app.pixelbadger.default_hostname}"
}

output "app_service_default_hostname" {
  description = "The default hostname of the App Service"
  value       = azurerm_linux_web_app.pixelbadger.default_hostname
}