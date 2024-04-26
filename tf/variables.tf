#--------------------------------------------------------------------
# Global variables
#--------------------------------------------------------------------
variable "do_token" {
  type      = string
  sensitive = true
}

variable "project_name" {
  description = "Name of DigitalOcean Project to group Resources into"
  type        = string
  default     = "EcommerceApp"
}

# Used to combine the tags provided along with the default tag needed and ensure no duplicates
locals {
  all_cluster_tags   = toset(concat(tolist(var.cluster_tags), [var.project_name]))
  all_node_pool_tags = toset(concat(tolist(var.node_pool_tags), [var.node_firewall_tag]))
  all_firewall_tags  = toset(concat(tolist(var.firewall_tags), [var.node_firewall_tag]))
}

#--------------------------------------------------------------------
# K8s Cluster variables
#--------------------------------------------------------------------
variable "cluster_name" {
  description = "Name of the k8s cluster"
  type        = string
  default     = "qa-cluster-01"
}

variable "cluster_region" {
  description = "The region to host the cluster in"
  type        = string
  default     = "nyc1"
}

variable "k8s_version" {
  description = "The version of kubernetes to use for the cluster"
  type        = string
  default     = "1.29.1-do.0"
}

variable "cluster_tags" {
  description = "Tags to apply to the cluster"
  type        = set(string)
  default     = ["k8s", "qa"]
}

variable "cluster_auto_upgrade_verison" {
  description = "The version of k8s to apply patch updates to on the cluster"
  type        = string
  default     = "1.29."
}



#--------------------------------------------------------------------
# Node Pool variables
#--------------------------------------------------------------------
variable "node_pool_name" {
  description = "Name of the node pool for the cluster"
  type        = string
  default     = "qa-cluster-autoscale-worker-pool"
}

variable "node_size" {
  description = "Droplet size of the droplets in the node pool"
  type        = string
  default     = "s-1vcpu-2gb"
}

variable "node_pool_min_size" {
  description = "Mininmum number of droplets to have in the node pool"
  type        = number
  default     = 1
}

variable "node_pool_max_size" {
  description = "Maximum number of droplets to have in the node pool"
  type        = number
  default     = 3
}

variable "node_firewall_tag" {
  description = "The tag used to map the firewall to the nodes"
  type        = string
  default     = "qa-cluster-01-nodes"
}

variable "node_pool_tags" {
  description = "Tags to apply to the node pool"
  type        = set(string)
  default     = []
}



#--------------------------------------------------------------------
# Database variables
#--------------------------------------------------------------------
variable "database_name" {
  description = "Name of the database"
  type        = string
  default     = "ecommerce"
}

variable "database_user" {
  description = "User account for applications to use when accessing the database"
  type        = string
  default     = "applicationuser"
}

variable "database_cluster_name" {
  description = "The name of the database cluster"
  type        = string
  default     = "qa-database-01-mysql"
}

variable "database_mysql_version" {
  description = "The version of mysql to use"
  type        = string
  default     = "8"
}

variable "database_instance_size" {
  description = "Size of the server running the database node"
  type        = string
  default     = "db-s-1vcpu-1gb"
}

variable "database_region" {
  description = "The region to host the database in"
  type        = string
  default     = "nyc1"
}

variable "database_node_count" {
  description = "The number of nodes in the database cluster"
  type        = number
  default     = 1
}


#--------------------------------------------------------------------
# Firewall variables
#--------------------------------------------------------------------
variable "firewall_name" {
  description = "The name of the firewall used for the cluster"
  type        = string
  default     = "qa-cluster-01-firewall"
}

variable "firewall_tags" {
  description = "The tags to apply to the firewall"
  type        = set(string)
  default     = []
}