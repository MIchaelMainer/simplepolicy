package simplepolicy.GET.me
default allowed = false

# only allow myself
allowed {
  input.user.id == "e424801b-531e-4ec8-bcbe-00fe6d5aa5d3"
}