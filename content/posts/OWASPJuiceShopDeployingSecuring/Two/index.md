---
Title: Deploying Then Securing the OWASP Juice Shop, Part Two of ? 
Lead: Manual deployment to AWS
date: 2023-08-03
draft: false
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

The [OWASP](https://owasp.org/www-project-juice-shop/) [Juice Shop](https://github.com/juice-shop/juice-shop) project itself suggests a way to deploy the application.  Basically, stand up an [EC2](https://aws.amazon.com/ec2/) instance, pull in the OJS' Docker image, and fire it up.  The fact that it has a Docker image also means (so far as I can tell at this point) that there are _at least_ three more possible ways to deploy the application into AWS, using [Lightsail](https://aws.amazon.com/lightsail/), [Elastic Beanstalk](https://aws.amazon.com/elasticbeanstalk/) or [App Runner](https://aws.amazon.com/apprunner/).[^whatsthedifference],[^actuallyfour]  There's also a [Vagrantfile](https://developer.hashicorp.com/vagrant/docs/vagrantfile), meaning that standing up a full self-contained VM for it should be reasonably simple, too—although, that's not _really_ Vagrant's intended use case.[^novagrant]  At the very least, the Vagrantfile should give a pretty good guide as to what you should do yourself to configure a fresh [Amazon Machine Instance](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/AMIs.html) to make it work with the OJS.  

[^whatsthedifference]:  To be completely honest, as at the time of writing I can't actually tell what the important differences are between Lightsail using containers, Elastic Beanstalk and App Runner.  Except that the first two qualify for a free tier for the first 12 months after account creation (transitively via Beanstalk, apparently), while the latter seemingly has no free tier.  I haven't worked with any of them in depth yet, though.
[^actuallyfour]:  Actually, at least four.  I think.  There's also the [Elastic Container Service](https://docs.aws.amazon.com/ecs/index.html).  As with the other three, though, I'm not too sure at this point what the differences are.
[^novagrant]:  In fact, I couldn't find any reference to Vagrant in AWS' official documentation, and almost nothing mentioning Vagrant in the AWS Marketplace.  Mitchell Hashimoto himself seemingly used to provide an AWS plugin to Vagrant, but that has apparently been deprecated now, with no indication of any sort of replacement or alternative.

The OJS itself, independent of deployment/development environment helpers, is a Node.js application.[^nodeversion]  If one really wants to perform the whole process oneself (rather than bundling everything up into a container or VM), it is one of the easiest languages/ecosystems to do-it-yourself with.  Perhaps following the Vagrantfile, as mentioned above.  One mildly interesting aspect of OJS is that it does _not_ require a separate database.  I haven't dug into the codebase much yet (nor do I have all that much experience with non-trivial Node.js), but it seems, best as I can tell, that it incorporates SQLite into itself.  E.g. OJS v15.0.0 lists `sqlite3`, with versions equal to or above 5.0.8, as part of its dependencies in its package.json file.  This does make deploying it much easier, but isn't wholly realistic to most real-life web applications, which usually use one or more external DBMSes.[^dockercomposedbms]

[^nodeversion]:  I targeted Node.js 18, since it was listed as the latest version of Node.js that was still under support by both Node.js and OJS.  I actually suspect the table listing the supported versions that I saw was probably out-of-date, but I figured I should just stick with the documentation.  Plus, the Dockerfile in the OJS release I'm using still specifies the use of Node.js 18 images.
[^dockercomposedbms]:  An excellent example of a well-put-together Docker Compose system for this sort of thing can be found in a [Containerised MISP distribution](https://github.com/NUKIB/misp) created by what I understand to be the Czech equivalent of the US' CISA.  It includes not only a running MISP, but the relevant backing MySQL and Redis containers, too.  I used it for some local testing against a live MISP instance while at Cosive, and it worked really well. 

## First Look

Before trying to deploy OJS online, it seems like a good idea to try to run it locally and get some impression of what a running instance should look like.  To that end, I decided to download a copy of the official Docker image, and run that according to the provided instructions.  A simple pull of the image and a `docker run --rm -p 3000:3000 bkimminich/juice-shop:v15.0.0` later, I have OJS running locally.  Et voilà 🎉

{{< figure src="OWASP_Juice_Shop_start.png" title="The OWASP Juice Shop front page on start up." alt="An image displaying the front page with of the OWASP Juice Shop, overlaid with a pop-up box explaining the purpose of the application." >}}

For now, I'm _not_ going to attempt to poke any holes into it.  That will come later once I have done a few different deployment methods.  I did find the scoreboard, though, which I understand is considered the very first challenge of the whole thing.  That particular challenge was simple enough that even I could do it without help (and a penetration tester I am _not_).

## EC2 Instance

For my first ~~trick~~ deployment, I figure I will just follow the instructions provided by the [OJS project itself](https://github.com/juice-shop/juice-shop/tree/master#amazon-ec2-instance) to deploy onto an EC2 instance.  They seem to be pretty simple & straightforward.  Though, since I'm not an AWS expert, the number of configuration options for a new EC2 instance seem somewhat overwhelming and perplexing.  For my purposes, I'm going to assume that the defaults will do if the OJS instructions haven't mentioned changing them.  This isn't completely a reasonable assumption, since things change in AWS all the time, but let's hope it is OK for this.

### Not Quite There

The instructions were pretty good (as good as you could reasonably expect for a free, third-party project).  Unfortunately, I think I fell down on the part about allowing inbound HTTP traffic in the security groups.

#### Second Time's the Charm

So I went back and re-did the deployment, this time ensuring that I assigned a security group with a rule allowing all inbound IPv4 HTTP traffic.  After a couple of minutes or so, I could indeed connect to the instance and start poking around in it.  I confirmed that I could still get to the scoreboard, and that I hadn't somehow had the same user that I registered in the local instance come across to the online instance (I have zero clue how that could happen beyond sorcery, but I figured I should try it just in case).

So that first go can be declared a success! 🥳  I terminated the instance moments later, of course, since I had done what I needed to do with it.  Or rather, I had copy-pasted what I needed from the OJS instructions into the relevant box on the instance launcher page.  Specifically, this:

```shell
#!/bin/bash
yum update -y
yum install -y docker
service docker start
docker pull bkimminich/juice-shop
docker run -d -p 80:3000 bkimminich/juice-shop
```

{{< figure src="OJS_EC2_Instance_Creation_User_Script.png" title="Setting the commands to run when the EC2 instance is launched." alt="A screenshot showing some of the settings for a new AWS EC2 instance.  In particular, this screenshot shows the commands to be run when the instance launches." >}}

Either way, it was running in the cloud.

## Lightsail

Next, I decided I would try to do a deployment via AWS' Lightsail service.  To the best of my understanding, it pretty much lets you do the same as what was involved in setting up the EC2 instance described above, but with many of the knobs simply set to reasonable defaults that should serve most purposes.  In particular, I _think_ it one uses Lightsail, it takes care of handling SSL/TLS certificate provision for you (I presume via [Let's Encrypt](https://letsencrypt.org/)), so I believe I should be able to talk to an instance of the OJS via HTTP**S**, rather than plain HTTP.  Which is very much to be favoured, given that security is a primary concern of this whole project.  Also, since we have a pre-made container image, we might as well re-use that and focus on using Lightsail's support for containers, rather than try fiddling to configure a full instance.

According to the [overview of Lightsail's container service](https://lightsail.aws.amazon.com/ls/docs/en_us/articles/amazon-lightsail-container-services), not only does it handle setting up HTTPS for you, but it also handles load balancing across running instances of the container.  Essentially, at the most basic, you just configure it to say how powerful you want the compute nodes to be, and how many running instances you want operating behind the load balancer, and it handles the rest of the process.  It's not cheap, however (emphasis added).

> The monthly price of your container service is calculated by multiplying the price of its power with the number of its compute nodes (the scale of your service). For example, a service with a medium power, which has a price of $40 USD, and a scale of 3 compute nodes, will cost $120 USD per month. **You are charged for your container service whether it's enabled or disabled, and whether it has a deployment or not.** You must delete your container service to stop being charged for it.

It's a good thing I'm not planning to leave an instance of this lying around!  AWS provides some Lightsail services on the 12-month free tier, but I don't fancy suddenly receiving a USD$120 charge out of the blue in a year's time...  In the context of the OJS, where – if I even wanted it deployed the cloud – I could get away with a single low-power instance, I think I'd choose the EC2 approach described above.  I can see the utility to Lightsail for certain (probably not-very-technical) users and use cases though.  Also, if I'm understanding the [pricing page](https://aws.amazon.com/lightsail/pricing/) correctly, it seemingly turns out that the Lightsail container service isn't normally available under the free tier.  Happily, as at the time of writing they do give you three months free on their Micro power level, with a scale of 1 node.  Given that I don't plan to keep this service around for longer than (at most) half an hour, that should do for my ends.

### Deployment

It turns out that the main 'getting started' docs for Lightsail's container service actually seem to be pretty good (unlike [some other](../onepointfive) getting started docs I could mention...).  Most notable is this section:

> Create a container service with a deployment if you plan to use a container image from a public registry. Otherwise, create your service without a deployment if you plan to use a container image that is on your local machine. You can push the container image from your local machine to your container service after your service is up and running. Then you can create a deployment using the pushed container image that is registered to your container service.

Given that I'm planning just to deploy the public OJS image already found [on Docker Hub](https://hub.docker.com/r/bkimminich/juice-shop), I'll set the deployment up _before_ creating the service.  I'm not 100% sure about what the correct configuration is, but I'll make my best guess based on the provided deployment instructions for the EC2 instance.  Which makes it seem like the whole thing is pretty self-contained, and all I probably _really_ have to do is ensure that requests are sent to port 3000.

{{< figure src="OJS_Lightsail_Container_service_deployment_1.png" title="My first best guess at the correct deployment configuration" alt="An image showing a configuration interface for setting up a Lightsail container service deployment." >}}

Even for something as relatively simple as the OJS, deployment takes a good couple of minutes or so.  Interestingly, attempts to navigate to the assigned semi-random URL return a 503 error with the message "Service Temporarily Unavailable" if you try to navigate to the page before the deployment has finished.  Happily, it appears that I got the deployment configuration correct!  Once the container service status rolls over to 'Running', I can access an instance of the Juice Shop online.  Taking a quick squiz at the CPU utilisation logs, it appears that the service faced an initial spike to around 40% of total available CPU around start up time (possibly when I first accessed the site(?)), but quickly dropped back to much lower—with the next highest peak (probably when I left a fake feedback comment) at a mere 15%.  Similarly, memory use spiked shortly after launch to 20.56% of the available amount, and then hit a steady state of around 17%-19%.  I'm unconvinced that a single Micro instance has the grunt to power an _actual_ e-commerce site, but it seems to do the trick for a single-user exemplar.  Of course, moments later I shut the whole thing down and deleted the service, since I don't want to be charged for it!

{{< figure src="OJS_Lightsail_Instance_CPU_util.png" title="CPU utilisation when running the OJS in a Lightsail container service" alt="A chart showing movements over time in the utilisation of the virtual CPU in the Lightsail container instance." >}}
{{< figure src="OJS_Lightsail_Instance_Mem_util.png" title="Memory utilisation when running the OJS in a Lightsail container service" alt="A chart showing movements over time in the utilisation of the allocated memory in the Lightsail container instance." >}}

### Lightsail Instance, Without the Container Service?

Given that there's (almost) no such thing as a free tier in the Lightsail Container Service, can I deploy the OJS to a regular Lightsail instance, and thus hopefully enjoy the usual free tier rate?  Perhaps it can be treated similarly to the EC2 instance, where basically the only thing done is to ensure that Docker is installed, pull the OWASP Juice Shop image and run it, then check the exposed endpoint a few minutes later.  Possibly if I just use the Amazon Linux AMI again (as done above), it'll all work.  Considering the performance metrics discussed above, I'll just go with the smallest of the free tier options, since it seems like that should cover it, and I'd prefer to minimise any surprise bills down the line.  Let's see how this goes...

The Lightsail instance itself gets up and running _much_ more quickly.  That probably reflects, however, the fact that there is a lot more work to be done on the initial startup compared to the container service deployment, which apparently handled absolutely everything for us.  Despite waiting a fair number of minutes, I'm unable to access the OJS via the IPv4 address that AWS assigned.  Eventually, I twig that half the problem is probably that the OJS listens on port 3000, and I'm trying to contact it on port 80 (i.e. the norm for HTTP).  Except, then I remember that it should just be running a Docker container, where port forwarding has been set up between ports 80 and 3000.  Just in case, I still try opening up port 3000 in the networking tab of the instance status page, and attempt to point the browser there.  No luck, however.

Furthermore, all attempts to SSH to the server fail, both by using AWS' own browser-based SSH, and trying to use the Windows 11 built-in SSH client.  Basically, in neither case does it seem like the server actually responds, although nothing times out either, so I'm not too sure what is really happening.  I can see on the CPU utilisation graph that it peaks at about 85% roughly 20 minutes into the deployment time, but still no luck connecting.  Attempts to navigate to the provided IP address all just a 'connection reset' error, and attempts to SSH in never resolve, and I have confirmed that port 22 is listed as open.  Most curiously, when I check the measure of incoming network traffic, there's about 30 MB listed on the first measurement after start up, but absolutely zero after that.  Given that I thought the Docker container was considerably larger than that, it looks to me like perhaps the instance never got to the point of downloading it.  There's no instance of status check failures, however, so it seems like probably _something_ in the deployment is still live and responding.  The exact same thing seems to continue after rebooting the instance.  I'm not sure what the problem was (though I assume I didn't configure everything correctly), but this experiment should probably be regarded as a failure.  To make matters worse, when I try to stop the instance AWS bills me $0.51.[^monthlybill]  The instance takes several minutes to stop, though, leading me to wonder if there was somehow and infinite loop or something going on inside.

[^monthlybill]:  Turned out that was actually the total charges for the previous month, and the timing was just an incredible coincidence.  The charge appears to suggest extremely strongly that one of the tutorials I did which claimed to be entirely on the free tier actually wasn't, though...

### Lightsail Instance, Without the Container?

The previous experiment more-or-less tried to replicate the EC2 deployment, but in a Lightsail instance.  Something about that, however, didn't work.  Perhaps AWS put something in to stop people from essentially replicating their Lightsail container service on the cheaper regular instances.  I don't know.  There are also instructions on the OJS GitHub page about building and running the whole thing from source or by downloading and unpacking a prepackaged release onto an environment with Node.js already installed.  It appears that Bitnami packages a Node.js AMI that can be used in Lightsail, so maybe trying these instructions using that will work better.

{{< figure src="OJS_Lightsail_Instance_Creation_Choose_Instance_Image.png" title="Bitnami's Node.js AMI for use in Lightsail" alt="A screenshot depicting the selection of Bitnami's Node.js Amazon Machine Image onto which to deploy an instance of the OWASP Juice Shop." >}}

#### From Source

This time around nobody has provided a ready-to-copy code block, but the OJS instructions are still fairly detailed.  It shouldn't be too difficult to turn those into a startup script.  Which ends up looking like this (at least for my first attempt):

```shell
git clone https://github.com/juice-shop/juice-shop.git --depth 1
cd juice-shop
npm install
npm start
```

This time around, there is no container port forwarding, so I probably will indeed need to access the site via port 3000.  Thus, I probably need to go expose port 3000 through the firewall again.  After doing so, I checked the metrics, and was surprised to see very little in the way of CPU utilisation and incoming network traffic.  It looks like, for some reason unclear to me, the startup script I supplied doesn't seem to have worked.  This time, however, when I try to SSH into the machine, it actually responds and I can poke around in the remote terminal.  There's no sign of the git repository in the home directory, so perhaps things fell at the first hurdle.  Let's see if I can do it all manually, at least.

Actually, after running `git clone`, I noticed that the total size of the repo appeared to be approximately the same as the amount of the incoming network traffic.  It turns out that the OJS repo was cloned under the top-level directory (i.e. `/`), but I suspect the other commands might well have been run from within the home directory.  In the future, I'll have to try to ensure that I clone things to the home directory specifically.  Now to wait while `npm install` does its thing and downloads approximately 1 million packages.  A few minutes later, it aborts with an out-of-memory error.  Presumably that was the _real_ reason why nothing happened.  Right, seems that if I want to build the application locally, I'll need more than 512 MB.  I'm not surprised, but I did kinda hope.  Ah well, I'll try to upgrade this instance's scale to the next level of the free tier.

This time around, I went with the largest of the free tier options (which has four times the RAM), but since I'm upgrading based on a snapshot of the previous deployment, I don't bother to set up the initial deployment script again.  Instead, I'll just SSH in again and continue the process manually.  Except, I then get an error telling me that I'm not allowed to create a new instance from the snapshot with that size.  So I go with the middle option, that only has double the RAM.  Hopefully, that'll still be enough.

This time around, the installation process doesn't fail with an out-of-memory error.  Instead, there are a bunch of deprecation warnings (which I assume are a deliberate part of making OJS an _intentionally_ vulnerable web application), and then a failing error which I don't quite understand.  I have included it below in case someone more familiar with Node.js than me can explain what it is about.

```shell
npm ERR! code 1
npm ERR! path /home/bitnami/juice-shop/node_modules/cypress
npm ERR! command failed
npm ERR! command sh -c node index.js --exec install
npm ERR! /home/bitnami/juice-shop/node_modules/isexe/index.js:25
npm ERR!         if (er
npm ERR!               
npm ERR! 
npm ERR! SyntaxError: Unexpected end of input
npm ERR!     at internalCompileFunction (node:internal/vm:73:18)
npm ERR!     at wrapSafe (node:internal/modules/cjs/loader:1178:20)
npm ERR!     at Module._compile (node:internal/modules/cjs/loader:1220:27)
npm ERR!     at Module._extensions..js (node:internal/modules/cjs/loader:1310:10)
npm ERR!     at Module.load (node:internal/modules/cjs/loader:1119:32)
npm ERR!     at Module._load (node:internal/modules/cjs/loader:960:12)
npm ERR!     at Module.require (node:internal/modules/cjs/loader:1143:19)
npm ERR!     at require (node:internal/modules/cjs/helpers:110:18)
npm ERR!     at Object.<anonymous> (/home/bitnami/juice-shop/node_modules/which/which.js:7:15)
npm ERR!     at Module._compile (node:internal/modules/cjs/loader:1256:14)
npm ERR! 
npm ERR! Node.js v18.16.1
```

It does occur to me that I forgot to set the cloned git repository to the relevant commit/tag, so I go back and ensure that I have checked out the `v15.0.0` tag and try `npm install` again.  I hit the exact same result in the end though.  To be honest, I can't be bothered trying to debug this any further right now, so I'll just move on to trying to install a pre-packaged release.

#### From A Release

The OJS team publish [pre-packaged releases](https://github.com/juice-shop/juice-shop/releases/tag/v15.0.0) of the project, as well as things like the Docker images.  By my estimation, given that the version of Node currently packaged into the Bitnami AMI is v18, I think I want the v15.0.0 node18 Linux .tgz.  Given that I already have a working Node.js Lightsail compute instance from the attempt to install from source, I'm just going to re-use that.  The instructions for using one of the releases amount to executing the following commands (yes, it turns out I probably could have just extracted to the home directory and skipped the first level of `juice-shop`):

```shell
wget https://github.com/juice-shop/juice-shop/releases/download/v15.0.0/juice-shop-15.0.0_node18_linux_x64.tgz
tar -xzvf juice-shop-15.0.0_node18_linux_x64.tgz -C ~/juice-shop
cd ~/juice-shop/juice-shop-15.0.0
npm start
```

Let's see if this does any better.  The output from the terminal after running the start command appears to match that seen when using the Docker container, so that seems promising.  I first try to access the page by browsing straight to the provided IPv4 address (unlike with the container service, using a regular Lightsail instance doesn't seem to allocate a URL automatically).  Of course, this first hits the problem that it's HTTP-only, and my browser is set to HTTPS-only mode.  Overriding that and continuing to navigate to the address lands me on a welcome page for the Bitnami Node.js image.  Trying to navigate to the same IP address on port 3000 doesn't seem to get me anywhere.  Then I realise that I still haven't opened port 3000 in the network settings for the instance.  After I correct that, I can indeed access the OJS in the cloud again.  After about 5 or so minutes, however, I suddenly can't get the server to respond on port 3000 to anything.  I'm not too sure what happened, but the application seemingly stopped running of its own accord.  Started working fine again as soon as I restarted it, though.

{{< figure src="OJS_Lightsail_Instance_Bitnami_NodeJS_frontpage.png" title="The Bitnami Node.js introductory front page, seen when accessing the assigned IP address via HTTP on port 80." alt="A screenshot of the welcome page for the Bitnami Node.js AMI." >}}

Anyway, now that I have confirmed that I can run the OJS in a regular Lightsail container (completely manually, at least), I'll delete it and move on to trying the next approach.

## The Next Deployment Approach

To be finished...