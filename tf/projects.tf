resource "digitalocean_project" "EcommerceApp" {
  name = var.project_name
}

resource "digitalocean_project_resources" "EcommerceApp_Resources" {
  project = digitalocean_project.EcommerceApp.id
  resources = [
    digitalocean_database_cluster.database_01_mysql.urn,
    digitalocean_kubernetes_cluster.cluster_01.urn
  ]
}