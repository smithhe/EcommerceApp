
output "cluster_endpoint" {
  value = digitalocean_kubernetes_cluster.cluster_01.endpoint
}

output "cluster_token" {
  value = digitalocean_kubernetes_cluster.cluster_01.kube_config[0].token
}

output "cluster_ca_certificate" {
  value = digitalocean_kubernetes_cluster.cluster_01.kube_config[0].cluster_ca_certificate
}