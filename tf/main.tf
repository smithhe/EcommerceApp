terraform {
  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = "~> 2.0"
    }

    kubernetes = {}
  }
}

#--------------------------------------------------------------------
# Module Call
#--------------------------------------------------------------------
module "EcommerceApp" {
  source = "./EcommerceModule"

  //Required variables
  do_token               = var.do_token
  dockerhub_access_token = var.dockerhub_access_token
  local_ip               = var.local_ip

  //Edit any other variables in the module as you would like otherwise defaults will be used
  node_size = "s-2vcpu-2gb"
  node_pool_max_size = 2
}

#--------------------------------------------------------------------
# Providers
#--------------------------------------------------------------------
provider "digitalocean" {
  token = var.do_token
}

provider "kubernetes" {
  host                   = module.EcommerceApp.cluster_endpoint
  token                  = module.EcommerceApp.cluster_token
  cluster_ca_certificate = base64decode(module.EcommerceApp.cluster_ca_certificate)
}

#--------------------------------------------------------------------
# Variables
#--------------------------------------------------------------------
variable "do_token" {
  type      = string
  sensitive = true
}

variable "dockerhub_access_token" {
  description = "Access token for DockerHub to pull images from private repositories"
  type        = string
  sensitive   = true
}

variable "local_ip" {
  description = "Your local IP address to add to the firewall for the database"
  type        = string
}