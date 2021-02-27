function drawArchitecturalRulesChart() {
    let chartElement = document.getElementById('architectural_rules_chart');
    
    let chartConfig = {
        type: 'doughnut',
        data: {
            datasets: [{
                data: [
                    followedRules.length,
                    transgressedRules.length
                ],
                backgroundColor: [
                    'rgb(7, 163, 20)', 
                    'rgb(214, 16, 16)'
                ]
            }],
            labels: [
                'Followed Rules',
                'Transgressed Rules'
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            circumference: Math.PI,
            rotation: -Math.PI,
            legend: {
                position: 'bottom',
            },
            animation: {
                animateRotate: true
            }
        }
    };
    
    new Chart(chartElement, chartConfig);
};

function createElementWithClass(elem, className) {
    let div = document.createElement(elem);
    
    div.className = className;

    return div;
}

function getMediaDiv(rule, hasViolations) {
    let media = createElementWithClass('div', 'media');
    let mediaBody = createElementWithClass('div', 'media-body rule-description');
    let img = createElementWithClass('div', 'align-self-center background-icon rule-icon');
    let p = createElementWithClass('p', 'media-heading');

    if (hasViolations)
        img.classList.add('rule-icon-fail');
    else
        img.classList.add('rule-icon-check');
    
    let ruleDescription = `<b>Origin: </b>${rule.OriginLayer} `;
    ruleDescription += `<b>Operator: </b>${rule.RuleOperator} `;
    ruleDescription += `<b>Target: </b>${rule.TargetLayer}`;

    if (rule.RelationTypes != null && rule.RelationTypes.length > 0) {
        let types = '';

        for (let i = 0; i < rule.RelationTypes.length; i++) {
            types += (types.length > 0 ? ', ' : '') + rule.RelationTypes[i];                        
        }

        ruleDescription += `<br />Applied to: ${types}`;
    } else {
        ruleDescription += `<br />Applied to all relation types`;
    }

    p.innerHTML = ruleDescription;

    mediaBody.appendChild(p);
    media.appendChild(img);
    media.appendChild(mediaBody);

    return media;
}

function getExpandButton(buttonClassName, collapseDivId) {
    let buttonLink = createElementWithClass('a', buttonClassName);
    
    buttonLink.setAttribute('data-toggle', 'collapse');
    buttonLink.setAttribute('href', `#${collapseDivId}`);
    buttonLink.setAttribute('role', 'button');
    buttonLink.setAttribute('aria-expanded', 'false');
    buttonLink.setAttribute('aria-controls', collapseDivId);
    
    let img = createElementWithClass('div', 'align-self-center background-icon expand-button expand-plus-button');

    buttonLink.appendChild(img);

    return buttonLink;
}

function getExpandComponent(expandClassName, buttonClassName, 
    collapseDivId, buttonComplement, alterCollapseDiv) {

    let expandSection = createElementWithClass('div', expandClassName);
    let expandSectionRow = createElementWithClass('div', 'row');
    let expandSectionCol = createElementWithClass('div', 'col-12');    
    let expandSectionLink = getExpandButton(buttonClassName, collapseDivId);
    
    let collapseDiv = createElementWithClass('div', 'collapse multi-collapse row');
    let collapseColDiv = createElementWithClass('div', 'col-12');

    collapseDiv.setAttribute('id', collapseDivId);

    expandSectionCol.appendChild(expandSectionLink);
    expandSectionRow.appendChild(expandSectionCol);
    
    let complement = createElementWithClass('span', 'expand-complement');
    complement.innerText = buttonComplement;
    expandSectionLink.appendChild(complement);
    
    alterCollapseDiv(collapseColDiv);
    collapseDiv.appendChild(collapseColDiv);

    expandSection.appendChild(expandSectionRow);
    expandSection.appendChild(collapseDiv);

    return expandSection;
}

function getViolationListItem(violation) {
    let li = createElementWithClass('li', 'list-group-item');
    
    let relations = violation.NonConformingRelations;
    
    if (relations != null && relations.length > 0)
    {
        let relationsCount = document.getElementsByClassName('violation-relations').length + 1;
        let relationsCollapseDivId = `relations-collapse-${relationsCount}`;        
        let violationRelations = getExpandComponent('violation-relations', 'expand-relations', 
            relationsCollapseDivId, ` ${violation.Structure}`, function(collapseDiv) {
                
                let violationsList = createElementWithClass('ul', 'list-group list-group-flush');

                for (let i = 0; i < relations.length; i++) {
                    let targets = relations[i].Targets;
                    let type = relations[i].RelationType;
                    let listItem = createElementWithClass('li', 'list-group-item');

                    if (targets.length == 1) {
                        listItem.innerHTML = `<b>Type: </b>${type} - <b>Target: </b>${targets[0]}`;
                    } else {
                        listItem.innerHTML = `<b>Type: </b>${type}<br />`;
                        let targetList = createElementWithClass(
                            'ul', 'list-group list-group-flush');

                        for (let j = 0; j < targets.length; j++) {
                            let liTarget = createElementWithClass('li', 'list-group-item');
                            liTarget.innerText = targets[j];
                            targetList.appendChild(liTarget);
                        }

                        listItem.appendChild(targetList);
                    }

                    violationsList.appendChild(listItem);
                }

                collapseDiv.appendChild(violationsList);
        });

        li.appendChild(violationRelations);
    } else {
        li.innerText = violation.Structure;
    }
    
    return li;
}

function getViolationsDiv(violations) {
    let violationsCount = document.getElementsByClassName('rule-content').length + 1;
    let violationsCollapseDivId = `violations-collapse-${violationsCount}`;

    let buttonComplement = ' Rule Violations';
    let ruleViolations = getExpandComponent('rule-violations', 'expand-violations', 
        violationsCollapseDivId, buttonComplement, function(collapseDiv) {
            let structureList = createElementWithClass(
                'ul', 'list-group list-group-flush');

            for (let i = 0; i < violations.length; i++) {
                structureList.appendChild(getViolationListItem(violations[i]));
            }

            collapseDiv.appendChild(structureList);
    });
    
    return ruleViolations;
}

function insertRule(mainDiv, rule, violations)
{
    let hasViolations = violations != null && violations.length > 0;
    let ruleContent = createElementWithClass('div', 'rule-content');
    
    ruleContent.appendChild(getMediaDiv(rule, hasViolations))

    if (hasViolations)
    {
        ruleContent.appendChild(getViolationsDiv(violations))
    }

    mainDiv.appendChild(ruleContent);
}

function fillWithRules() {
    const mainDiv = document.getElementById('rule-list');
    
    for (let i = 0; i < transgressedRules.length; i++) {
        insertRule(mainDiv,
            transgressedRules[i].Rule, 
            transgressedRules[i].Violations);
    }

    for (let i = 0; i < followedRules.length; i++) {
        insertRule(mainDiv,followedRules[i], null);
    }
}

function getParentByClass(element, className) {
    if (element == null || element.classList.contains(className))
        return element;

    return getParentByClass(element.parentElement, className);
}

function addToggleEventButton(buttonClass) {
    let buttons = document.getElementsByClassName(buttonClass);

    for (let i = 0; i < buttons.length; i++) {
        buttons[i].addEventListener('click', function(event) {
            let element = event.srcElement;
            let link = getParentByClass(element, buttonClass);

            if (link != null)
            {
                let img = link.getElementsByClassName('expand-button')[0]

                if (img)
                {
                    if (img.classList.contains('expand-plus-button')) {
                        img.classList.remove('expand-plus-button');
                        img.classList.add('expand-minus-button');
                    } else {
                        img.classList.remove('expand-minus-button');
                        img.classList.add('expand-plus-button');
                    }
                }
            }
        }, false);
    }
}