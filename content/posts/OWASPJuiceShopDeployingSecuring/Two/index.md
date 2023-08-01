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

This post covers various attempts to deploy the OWASP Juice Shop (OJS) application on AWS.  Multiple approaches are trialled, with the comment element between them being that these are all fairly manual 'point-and-click' style methods.  Good for getting oneself up and running the first time, while getting to grips with AWS.  Not so good for reliable, reproducible deployments, however.  For the purposes of the remainder of this series of blog posts, I will be using OJS [v15.0.0](https://github.com/juice-shop/juice-shop/releases/tag/v15.0.0), which was the latest official release at the time I started.

## Possible Juice Shop Deployment Methods

The [OWASP](https://owasp.org/www-project-juice-shop/) [Juice Shop](https://github.com/juice-shop/juice-shop) project itself suggests a way to deploy the application.  Basically, stand up an [EC2](https://aws.amazon.com/ec2/) instance, pull in the OJS' Docker image, and fire it up.  The fact that it has a Docker image also means (so far as I can tell at this point) that there are _at least_ three more possible ways to deploy the application into AWS, using [Lightsail](https://aws.amazon.com/lightsail/), [Elastic Beanstalk](https://aws.amazon.com/elasticbeanstalk/) or [App Runner](https://aws.amazon.com/apprunner/).[^whatsthedifference]  There's also a [Vagrantfile](https://developer.hashicorp.com/vagrant/docs/vagrantfile), meaning that standing up a full self-contained VM for it should be reasonably simple, too—although, that's not _really_ Vagrant's intended use case.[^novagrant]  At the very least, the Vagrantfile should give a pretty good guide as to what you should do yourself to configure a fresh [Amazon Machine Instance](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/AMIs.html) to make it work with the OJS.  

[^whatsthedifference]:  To be completely honest, as at the time of writing I can't actually tell what the important differences are between Lightsail using containers, Elastic Beanstalk and App Runner.  Except that the first two qualify for a free tier for the first 12 months after account creation (transitively via Beanstalk, apparently), while the latter seemingly has no free tier.  I haven't worked with any of them in depth yet, though.
[^novagrant]:  In fact, I couldn't find any reference to Vagrant in AWS' official documentation, and almost nothing mentioning Vagrant in the AWS Marketplace.  Mitchell Hashimoto himself seemingly used to provide an AWS plugin to Vagrant, but that has apparently been deprecated now.

The OJS itself, independent of deployment/development environment helpers, is a Node.js application.[^nodeversion]  If one really wants to perform the whole process oneself (rather than bundling everything up into a container or VM), it is one of the easiest languages/ecosystems to do-it-yourself with.  Perhaps following the Vagrantfile, as mentioned above.  One mildly interesting aspect of OJS is that it does _not_ require a separate database.  I haven't dug into the codebase much yet (nor do I have all that much experience with non-trivial Node.js), but it seems, best as I can tell, that it incorporates SQLite into itself.  E.g. OJS v15.0.0 lists `sqlite3`, with versions equal to or above 5.0.8, as part of its dependencies in its package.json file.  This does make deploying it much easier, but isn't wholly realistic to most real-life web applications, which usually use one or more external DBMSes.[^dockercomposedbms]

[^nodeversion]:  I targeted Node.js 18, since it was listed as the latest version of Node.js that was still under support by both Node.js and OJS.  I actually suspect the table listing the supported versions that I saw was probably out-of-date, but I figured I should just stick with the documentation.  Plus, the Dockerfile in the OJS release I'm using still specifies the use of Node.js 18 images.
[^dockercomposedbms]:  An excellent example of a well-put-together Docker Compose system for this sort of thing can be found in a [Dockerised MISP distribution](https://github.com/NUKIB/misp) created by what I understand to be the Czech equivalent of the US' CISA.  It includes not only a running MISP, but the relevant backing MySQL and Redis containers, too.  I used it for some local testing against a live MISP instance while at Cosive, and it worked really well. 

## First Look

Before trying to deploy OJS online, it seems like a good idea to try to run it locally and get some impression of what a running instance should look like.  To that end, I decided to download a copy of the official Docker image, and run that according to the provided instructions.  A simple pull of the image and a `docker run --rm -p 3000:3000 bkimminich/juice-shop:v15.0.0` later, I have OJS running locally.  Et voilà 🎉

{{< figure src="OWASP_Juice_Shop_start.png" title="The OWASP Juice Shop front page on start up." alt="An image displaying the front page with of the OWASP Juice Shop, overlaid with a pop-up box explaining the purpose of the application." >}}

For now, I'm _not_ going to attempt to poke any holes into it.  That will come later once I have done a few different deployment methods.  I did find the scoreboard, though, which I understand is considered the very first challenge of the whole thing.  That particular challenge was simple enough that even I could do it without help (and a penetration tester I am _not_).

## EC2 Instance

For my first ~~trick~~ deployment, I figure I will just follow the instructions provided by the [OJS project itself](https://github.com/juice-shop/juice-shop/tree/master#amazon-ec2-instance) to deploy onto an EC2 instance.  They seem to be pretty simple & straightforward.  Though, since I'm not an AWS expert, the number of configuration options for a new EC2 instance seem somewhat overwhelming and perplexing.  For my purposes, I'm going to assume that the defaults will do if the OJS instructions haven't mentioned changing them.  This isn't completely a reasonable assumption, since things change in AWS all the time, but let's hope it is OK for this.

### Not Quite There

The instructions were pretty good (as good as you could reasonably expect for a free, third-party project).  Unfortunately, I think I fell down on the part about allowing inbound HTTP traffic in the security groups.

#### Second Time's the Charm

So I went back and re-did the deployment, this time ensuring that I assigned a security group with a rule allowing all inbound IPv4 HTTP traffic.  After a couple of minutes or so, I could indeed connect to the instance and start poking around in it.  I confirmed that I could still get to the scoreboard, and that I hadn't somehow had the same user that I registered in the local instance come across to the online instance (I have zero clue how that would have happened beyond evil sorcery, but I figured I should try it just in case).

So that first go can be declared a success! 🥳  I terminated the instance moments later, of course, since I had done what I needed to do with it.  Or rather, I had copy-pasted what I needed from the OJS instructions.  Specifically, this:

```shell
#!/bin/bash
yum update -y
yum install -y docker
service docker start
docker pull bkimminich/juice-shop
docker run -d -p 80:3000 bkimminich/juice-shop
```

Either way, it was running in the cloud.