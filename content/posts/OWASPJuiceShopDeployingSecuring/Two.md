---
Title: Deploying Then Securing the OWASP Juice Shop, Part Two of ? 
Lead: Manual deployment to AWS
date: 2023-07-28
draft: true
Tags:

- AWS
- Cloud
- Deployment
- OWASP Juice Shop
- OWASP

Series:
  - Deploying Then Securing the OWASP Juice Shop
---
# Deploying the Juice Shop to AWS, the manual way

This post covers various attempts to deploy the OWASP Juice Shop application on AWS.  Multiple approaches are trialled, with the comment element between them being that these are all fairly manual 'point-and-click' style methods.  Good for getting oneself up and running the first time, while getting to grips with AWS.  Not so good for reliable, reproducible deployments, however.  For the purposes of the remainder of this series of blog posts, I will be using Juice Shop [v15.0.0](https://github.com/juice-shop/juice-shop/releases/tag/v15.0.0), which was the latest official release at the time I started.

## Possible Juice Shop Deployment Methods

The [OWASP](https://owasp.org/www-project-juice-shop/) [Juice Shop](https://github.com/juice-shop/juice-shop) project itself suggests a way to deploy the application.  Basically, stand up an [EC2](https://aws.amazon.com/ec2/) instance, pull in the Juice Shop's Docker image, and fire it up.  The fact that it has a Docker image also means (so far as I can tell at this point) that there are _at least_ three more possible ways to deploy the application into AWS, using [Lightsail](https://aws.amazon.com/lightsail/), [Elastic Beanstalk](https://aws.amazon.com/elasticbeanstalk/) or [App Runner](https://aws.amazon.com/apprunner/).[^whatsthedifference]  There's also a [Vagrantfile](https://developer.hashicorp.com/vagrant/docs/vagrantfile), meaning that standing up a full self-contained VM for it should be reasonably simple, too—although, that's not _really_ Vagrant's intended use case.[^novagrant]  At the very least, the Vagrantfile should give a pretty good guide as to what you should do yourself to configure a fresh [Amazon Machine Instance](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/AMIs.html) to make it work with the Juice Shop.  

[^whatsthedifference]:  To be completely honest, as at the time of writing I can't actually tell what the important differences are between Lightsail using containers, Elastic Beanstalk and App Runner.  Except that the first two qualify for a free tier for the first 12 months after account creation (transitively via Beanstalk, apparently), while the latter seemingly has no free tier.  I haven't worked with any of them in depth yet, though.
[^novagrant]:  In fact, I couldn't find any reference to Vagrant in AWS' official documentation, and almost nothing mentioning Vagrant in the AWS Marketplace.  Mitchell Hashimoto himself seemingly used to provide an AWS plugin to Vagrant, but that has apparently been deprecated now.

The OWASP Juice Shop itself, independent of deployment/development environment helpers, is a Node.js application.[^nodeversion]  If one really wants to perform the whole process oneself (rather than bundling everything up into a container or VM), it is one of the easiest languages/ecosystems to do-it-yourself with.  Perhaps following the Vagrantfile, as mentioned above.  

[^nodeversion]:  I targeted Node.js 18, since it was listed as the latest version of Node.js that was still under support by both Node.js and Juice Shop.  I actually suspect the table listing the supported versions that I saw was probably out-of-date, but I figured I should just stick with the documentation.