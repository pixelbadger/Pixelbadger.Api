variable "workload" {
  description = "The name of the workload"
  type        = string
  default     = "pixelbadger"
}

variable "environment" {
  description = "The environment name (e.g., dev, staging, prod)"
  type        = string
  default     = "dev"
}

variable "location" {
  description = "The Azure region where resources will be created"
  type        = string
  default     = "East US"
}

locals {
  tags = {
    Environment = var.environment
    Workload    = var.workload
    ManagedBy   = "terraform"
  }
}